using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform leftCenter;
    [SerializeField] private Transform rightCenter;
    [SerializeField] private double angleSpeed = 10;
    [SerializeField] private double speed = 3;
    private Rigidbody rb;
    private TurnDir _dirTurn = TurnDir.Nope;
    private bool _isGameOver;
    private bool _isGameStarted;
    private bool _isGamePaused;
    private float _tempAngle;
    private Quaternion _cashRotate;
    private Vector3 _cashCenter;
    private Vector3 _dirRacing;

    public enum TurnDir
    {
        Nope,
        Left,
        Right
    }

    private void Awake()
    {
        GlobalEventManager.OnGameStart.AddListener(StartGame);
        GlobalEventManager.OnStartPause.AddListener(StartPause);
        GlobalEventManager.OnFinishPause.AddListener(StopPause);
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        _dirRacing = new Vector3((int)speed, 0, 0);
    }
    
    void Update()
    {
        if(_isGameOver || !_isGameStarted || _isGamePaused) return;
        
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            StopDriftTurn();
        }

        if (_dirTurn != TurnDir.Nope)
        {
            DriftTurn();
            return;
        }

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _dirTurn = TurnDir.Left;
                _cashCenter = leftCenter.position;
            }
            else
            {
                _dirTurn = TurnDir.Right;
                _cashCenter = rightCenter.position;
            }

            _cashRotate = gameObject.transform.rotation;
            _tempAngle = 0;
            DriftTurn();
            return;
        }

        MoveCar();
    }

    private void FixedUpdate()
    {
        if(gameObject.transform.position.y < -3)
            EndGame();
    }

    private void StartPause()
    {
        _isGamePaused = true;
        print("pause");
    }

    private void StopPause()
    {
        _isGamePaused = false;
        print("stop");
    }

    private void StartGame()
    {
        _isGameOver = false;
        _isGameStarted = true;
    }
    private void EndGame()
    {
        _isGameOver = true;
        rb.Sleep();
        GlobalEventManager.OnGameOver.Invoke();
    }

    private void StartDriftTurn(TurnDir dir)
    {
        _cashCenter = dir == TurnDir.Left ? leftCenter.position : rightCenter.position;

        _dirTurn = dir;
        _cashRotate = gameObject.transform.rotation;
        _tempAngle = 0;
        DriftTurn();
    }

    private void StopDriftTurn()
    {
        Debug.Log("f");
        gameObject.transform.rotation = _cashRotate;
        float rotate = (int) _tempAngle / 90 + 1;
        if (_dirTurn == TurnDir.Left) //для отрицатешльного поворота налево
            rotate *= -1;
        gameObject.transform.Rotate(0f,rotate * 90f , 0f);
        _dirTurn = TurnDir.Nope;
    }

    private void MoveCar()
    {
        gameObject.transform.Translate(_dirRacing * Time.deltaTime);
    }

    private void DriftTurn()
    {
        Vector3 axis;
        if (_dirTurn == TurnDir.Left)
        {
            axis = new Vector3(0, -1, 0);
        }
        else
        {
            axis = new Vector3(0, 1, 0);
        }
        gameObject.transform.RotateAround(_cashCenter, axis,
            (int) angleSpeed * Time.deltaTime);
        _tempAngle += (int) angleSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("ChunkPassed")) 
        {
            Destroy(other.gameObject);
            speed += 0.01f;
            angleSpeed += 0.5f;
            _dirRacing = new Vector3((int)speed, 0, 0);
            GlobalEventManager.OnChunkComplete.Invoke();
        }
        else if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            GlobalEventManager.OnMoneyAdd.Invoke();
        }
        else if(other.CompareTag("Obstacle"))
        {
            EndGame();
        }
    }
}
