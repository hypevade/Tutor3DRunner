using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GenerationScript : MonoBehaviour
{
   [SerializeField] private int maxCountSpawnedChunks = 30;
   //[SerializeField] private Rode startChunk;
   //[SerializeField] private Rode simpleRode;
   [SerializeField] private LeftTurn leftTurn;
   [SerializeField] private RightTurn rightTurn;
   [SerializeField] private List<Rode> chunksPrefabs;
   private int _xDelta = 0;
   private int _zDelta = 0;
   private DirTrace _dir;
   private Queue<BaseChunk> _spawnedChunks;


   enum DirTrace
   {
      Left,
      Right
   }

   public void Awake()
   {
      GlobalEventManager.OnChunkComplete.AddListener(SpawnNext);
      GlobalEventManager.OnChunkComplete.AddListener(DestroyСhunk);
   }

   public void Start()
   {
      _spawnedChunks = new Queue<BaseChunk>();
      _dir = DirTrace.Right;
      for (var i = 0; i < 20; i++)
      {
         SpawnNext();
      }
   }

   private void SpawnNext()
   {
      var tempRnd = Random.Range(1, 10);
      if (tempRnd % 3 == 0)
         SpawnTurn();
      else
         SpawnTrace();
   }

   private void SpawnTrace()
   {
      var original = chunksPrefabs[Random.Range(0, chunksPrefabs.Count - 1)];

      Vector3 newPos; 
      if (_dir == DirTrace.Left)
      {
         newPos = new Vector3(_xDelta, 0, _zDelta + 2);
         _zDelta += 2;
      }
      else
      {
         newPos = new Vector3(_xDelta + 2, 0, _zDelta);
         _xDelta += 2;
      }

      var nextRode = Instantiate(original,newPos,Quaternion.identity);
      if (_dir == DirTrace.Left)
         nextRode.transform.Rotate(0, 90, 0);
      
      _spawnedChunks.Enqueue(nextRode);
   }

   private void DestroyСhunk()
   {
      if (_spawnedChunks.Count < maxCountSpawnedChunks) 
         return;
      
      var temp = _spawnedChunks.Dequeue();
      Destroy(temp.gameObject);
   }

   private void SpawnTurn()
   {
      Turn original;
      Vector3 newPos;
      
      if (_dir == DirTrace.Left)
      {
         original = rightTurn;
         newPos = new Vector3(_xDelta, 0, _zDelta + 2);
         _zDelta += 2;
         _dir = DirTrace.Right;
      }
      else
      {
         original = leftTurn;
         newPos = new Vector3(_xDelta + 2, 0, _zDelta);
         _xDelta += 2;
         _dir = DirTrace.Left;
      }

      var spawnRode = Instantiate(original, newPos, Quaternion.identity);
      _spawnedChunks.Enqueue(spawnRode);
   }
}
