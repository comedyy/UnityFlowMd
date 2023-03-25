using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Mermaid : I_Mermaid {
   //Hard
   async UniTask I_Mermaid.A() {
       Debug.Log("AAA");
       await UniTask.Delay(1111);
       Debug.Log("AfterA");
   }

   //Round
   void I_Mermaid.B() {
       Debug.Log("B");
   }

   //Condition
   bool I_Mermaid.C() {
       return Random.value < 0.5f;
   }

   //Result 1
   void I_Mermaid.D() {
       Debug.Log("D");
   }

   //Result 2
   void I_Mermaid.E() {
       Debug.Log("E");
   }

   //O1
   void I_Mermaid.O1() {
       Debug.Log("O1");
   }

   //O2
   void I_Mermaid.O2() {
       Debug.Log("O2");
   }

   //O3
   void I_Mermaid.O3() {
       Debug.Log("O3");
   }

   //O4
   void I_Mermaid.O4() {
       Debug.Log("O4");
   }

   //O5
   void I_Mermaid.O5() {
       Debug.Log("O5");
   }

   //End
   void I_Mermaid.G() {
       Debug.Log("G");
   }

    public void CleanUp()
    {
    }
}
