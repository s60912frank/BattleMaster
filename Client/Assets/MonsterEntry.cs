using UnityEngine;
using System.Collections;

public class MonsterEntry : MonoBehaviour {
    public enum Direction
    {
        ToLeft,
        ToRight
    }
    public Direction dir = Direction.ToLeft;
    public float speed = 0.2f;
    public float distance = 50;
    private Vector3 finalPos;
	// Use this for initialization
	void Start () {
        finalPos = transform.position;
        if (dir == Direction.ToLeft)
            transform.position += distance * Vector3.right;
        else if(dir == Direction.ToRight)
            transform.position -= distance * Vector3.right;
	}
	
	// Update is called once per frame
	void Update () {
        if (dir == Direction.ToLeft && transform.position.x - finalPos.x > 0)
            transform.position -= speed * Vector3.right;
        else if (dir == Direction.ToRight && transform.position.x - finalPos.x < 0)
            transform.position += speed * Vector3.right;
	}
}
