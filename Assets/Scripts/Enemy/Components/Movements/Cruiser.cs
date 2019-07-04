using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cruiser : BaseMover
{
    public override void Begin(float wait = 0)
    {
        if (!isRotSet) SetRotRate(3f);
        base.Begin(wait);
    }

    protected virtual Vector3 GetPosition(float time)
    {
        return Vector3.zero;
    }

    protected override IEnumerator Move(float wait)
    {
        yield return new WaitForSeconds(wait);

        float time = 0f;
        float rate = 0f;
        Vector3 origin = transform.position;

        while (isExcuting)
        {
            Vector3 dir = Player.instance.transform.position - transform.position;
            dir.y = 0f;
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rot * Time.deltaTime);

            Vector3 target = origin + GetPosition(time);
            Vector3 offset = target - transform.position;
            
            if (parent.controller != null)
                parent.controller.Move(offset);
            else
                transform.Translate(offset, Space.World);

            yield return null;
            if (rate < 1f)
            {
                rate += Time.deltaTime;
                rate = Mathf.Clamp(rate, 0f, 1f);
            }
            time += Time.deltaTime * rate;
        }
    }
}