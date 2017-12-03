using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance { get { return _instance = Instance ?? FindObjectOfType<GameManager>(); } }
   private static GameManager _instance;

   private void Start()
   {

   }

   private void Update()
   {

   }
}
