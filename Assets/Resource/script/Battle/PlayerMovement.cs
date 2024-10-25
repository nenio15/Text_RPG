using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerUiManager playerUiManager;
    [SerializeField] private PlayerHealth characterData;

    [SerializeField] private GameObject Enemy;
    public float speed = 0.5f;

    private RectTransform tr;
    public RectTransform target;

    //private Transform tr;
    //public Vector3 target;
    public Vector3 pos;

    public bool run = false;
    //State state = State.Stop;

    private Rigidbody playerRigidbody;
    private Animator playerAnimator;

    void Awake()
    {
        characterData = gameObject.GetComponent<PlayerHealth>();


        tr = GetComponent<RectTransform>();
        target = Enemy.GetComponent<RectTransform>();
        //pos = target;

        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

    }

    //���� ���� �ֱ⿡ ���� ����
    private void FixedUpdate()
    {
        if (run) Move();

        //playerAnimator.SetFloat("Move", speed);

    }

    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, speed);
        if(Vector3.Distance(transform.position, pos) < 100) run = false;

        //��. �������� �����ѰŴ� �ƴѵ�.
        //playerRigidbody.MovePosition(transform.position + a);
    }

    //��׷� ��󿡰� �����ϱ�.
    IEnumerator UpdateRun(RectTransform target)
    {
        Debug.Log("move");
        float distance = Vector3.Distance(transform.position, target.position);
        if(distance < 100) { StopCoroutine(UpdateRun(target)); }

        while (distance >= 100) //trigger is you now... hmm
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed);
            yield return new WaitForSeconds(0.001f);

            //Vector3 move = new Vector3(pos.x - tr.position.x, pos.y - tr.position.y, 0).normalized * speed * Time.deltaTime; //target.position.x - tr.position.x, target.position.y - tr.position.y, 0
            //tr.position += move;
            //yield return null;
        }
    }

    /*
    public void UpdateRun(Vector3 pos) //IEnumerator UpdateRun(Vector3 pos) //�̰� �� ienumerator�� �ߴ���....
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, speed);

        //Vector3 move = new Vector3(pos.x - tr.position.x, pos.y - tr.position.y, 0).normalized * speed * Time.deltaTime;
        //tr.position += move;

    }
    */


}
