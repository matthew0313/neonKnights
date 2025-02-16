using UnityEngine;

public class HitscanBullet : HitscanEffect
{
    [SerializeField] float speed = 40.0f;
    float range;
    public override void Set(float range)
    {
        counter = 0.0f;
        this.range = range;
    }
    float counter = 0.0f;
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
        counter += speed * Time.deltaTime;
        if (counter >= range) Release();
    }
}