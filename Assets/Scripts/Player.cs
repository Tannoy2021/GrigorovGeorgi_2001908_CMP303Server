using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int id;
    public CharacterController controller;
   // public Rigidbody rigidbody;
    public Text textMessage;
    public float gravity = -9.81f;
    public bool shiftSpeedBoost = false;
    public float moveSpeed = 5f;
    private bool[] clientInputs;
    private float yVelocity = 0;
    public int itemAmount = 0;
    public int maxItemAmount = 7;
    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        //rigidbody = GetComponent<Rigidbody>();
    }
    public void Initialize(int _id)
        {
            id = _id;
           clientInputs = new bool[3];
        }
        public void FixedUpdate()
        {  
            Vector2 _inputDirection = Vector2.zero;
            shiftSpeedBoost = false;
            if (clientInputs[0])
            {
                _inputDirection.y += 1;
            }
            if (clientInputs[1])
            {
                _inputDirection.y -= 1;
            }
            if (clientInputs[2])
            {
                shiftSpeedBoost = true;
            }
        Move(_inputDirection);
        }
        private void Move(Vector2 _inputDirection)
        {   
            Vector3 _moveDirection = transform.right * _inputDirection.x + transform.forward * _inputDirection.y;
        if (shiftSpeedBoost == true)
        {
            _moveDirection *= moveSpeed * 2;
        }
        if(shiftSpeedBoost == false)
        {
            _moveDirection *= moveSpeed;
        }

        if(controller.isGrounded)
        { 
            yVelocity = 0;
        }
        yVelocity += gravity;
        _moveDirection.y = yVelocity;

        controller.Move(_moveDirection);
        //rigidbody.MovePosition(new Vector3 (_moveDirection.x,_moveDirection.y,_moveDirection.z));
        //rigidbody.MovePosition(_moveDirection);
            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }  
        public void SetInput(bool[] _inputs,Quaternion _rotation)
        {
            clientInputs = _inputs;
            transform.rotation = _rotation;
        }
    public bool AttemptPickup()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }
        itemAmount++;
        moveSpeed = moveSpeed + 0.08f;
        StartCoroutine(SpeedBuff());
        return true;
    }
    public bool PlayerFinish()
    {
        Debug.Log($" Player {id} finished in : ");
        ServerSend.PlayerPosition(this);
        controller.enabled = false;
        StartCoroutine(FinishTest());     
        return true;
    }
    //private IEnumerator DisplayMessage(string message)
    //{
    //    textMessage.text = message;
    //    yield return new WaitForSeconds(2f);
    //    textMessage.text = "Winner";
    //}
    private IEnumerator FinishTest()
    {
        yield return new WaitForSeconds(2f);

    }
    private IEnumerator SpeedBuff()
    {
        yield return new WaitForSeconds(3f);
        moveSpeed = moveSpeed - 0.08f;
        if(itemAmount > 0)
        {
            itemAmount--;
        }
    }

}

