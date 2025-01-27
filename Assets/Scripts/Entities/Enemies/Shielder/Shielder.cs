using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Enemy
{
    [SerializeField] float detectRange;
    [SerializeField] float rotateSpeed, rotateTime;
    class ShielderFSMVals : FSMVals
    {

    }
    class Shielder_TopLayer : TopLayer<Shielder, ShielderFSMVals>
    {
        public Shielder_TopLayer(Shielder origin, ShielderFSMVals values) : base(origin, values)
        {
            
        }
        class Idle : State<Shielder, ShielderFSMVals>
        {
            public Idle(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
            {

            }
            public override void OnStateUpdate()
            {
                base.OnStateUpdate();
                if(Vector3.Distance(origin.transform.position, origin.player.transform.position) <= origin.detectRange)
                {
                    parentLayer.ChangeState("Detected");
                }
            }
        }
        class Detected : Layer<Shielder, ShielderFSMVals>
        {
            public Detected(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
            {

            }
            class Rotating : State<Shielder, ShielderFSMVals>
            {
                public Rotating(Shielder origin, Layer<Shielder, ShielderFSMVals> parent) : base(origin, parent)
                {

                }
                float counter = 0.0f;
                public override void OnStateEnter()
                {
                    base.OnStateEnter();
                    counter = 0.0f;
                }
                public override void OnStateUpdate()
                {
                    base.OnStateUpdate();
                    Vector3 tmp = origin.player.transform.position - origin.transform.position;
                    float deg = Mathf.Atan2(tmp.z, tmp.x) * Mathf.Rad2Deg;
                    origin.transform.rotation = Quaternion.Slerp(origin.transform.rotation, Quaternion.Euler(0, deg, 0), origin.rotateSpeed * Time.deltaTime);
                    counter += Time.deltaTime;
                    if(counter > origin.rotateTime)
                    {
                        parentLayer.ChangeState("Dashing");
                    }
                }
            }
        }
    }
}