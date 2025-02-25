using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVCamera : MonoBehaviour, ISavable, ICutsceneCallbackReceiver
{
    [SerializeField] CCTVSequence[] sequence;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] SaveID id;
    int index = 0;
    float counter = 0.0f;
    bool arrived = false;

    bool paused = false;

    private void Update()
    {
        if (paused) return;
        if (!arrived)
        {
            float dist = Vector3.Distance(transform.position, sequence[index].wayPoint.position);
            if (dist <= 0.01f)
            {
                arrived = true;
            }
            else
            {
                transform.Translate((sequence[index].wayPoint.position - transform.position).normalized * Mathf.Min(moveSpeed * Time.deltaTime, dist));
            }
        }
        else
        {
            counter += Time.deltaTime;
            if (counter >= sequence[index].waitTime)
            {
                counter = 0.0f;
                arrived = false;
                if (++index >= sequence.Length) index = 0;
            }
        }
    }
    public void Save(SaveData data)
    {
        DataUnit tmp = new();
        tmp.floats["posX"] = transform.position.x;
        tmp.floats["posY"] = transform.position.y;
        tmp.floats["posZ"] = transform.position.z;
        tmp.integers["index"] = index;
        tmp.bools["arrived"] = arrived;
        tmp.floats["counter"] = counter;
        data.mapElements[id.value] = tmp;
    }
    public void Load(SaveData data)
    {
        if(data.mapElements.TryGetValue(id.value, out DataUnit tmp))
        {
            Vector3 pos = Vector3.zero;
            if (tmp.floats.TryGetValue("posX", out float posX)) pos.x = posX;
            if (tmp.floats.TryGetValue("posY", out float posY)) pos.y = posY;
            if (tmp.floats.TryGetValue("posZ", out float posZ)) pos.z = posZ;
            transform.position = pos;

            tmp.integers.TryGetValue("index", out index);
            tmp.bools.TryGetValue("arrived", out arrived);
            tmp.floats.TryGetValue("counter", out counter);
        }
    }

    public void OnCutsceneStart()
    {
        paused = true;
    }

    public void OnCutsceneEnd()
    {
        paused = false;
    }
}
[System.Serializable]
public struct CCTVSequence
{
    [SerializeField] Transform m_wayPoint;
    [SerializeField] float m_waitTime;
    public Transform wayPoint => m_wayPoint;
    public float waitTime => m_waitTime;
}
