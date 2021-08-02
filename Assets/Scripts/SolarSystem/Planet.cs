using UnityEngine;
using Unity.Mathematics;

public class Planet : MonoBehaviour
{
    public PlanetPath path;
    private float t = 0f;

    // Update is called once per frame
    void Update()
    {
        var index = (int)math.floor(this.t % path.points.Length);
        var t =  (math.floor(this.t % path.points.Length) - (float)index) / ((float)index + 1);
        var p = math.lerp(path.points[index], path.points[(index + 1) % path.points.Length], t);
        transform.position = new Vector3(p.x, p.y, 0);

        this.t += Time.deltaTime;
    }
}
