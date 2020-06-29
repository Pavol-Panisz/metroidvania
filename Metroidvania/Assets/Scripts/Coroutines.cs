#region README
/* A class for handling coroutines. Ideally, I'd like to have
 * the capability of writing coroutines.WaitThenExecute(..) in
 * any script, but that would require this to be a static class,
 * which it cannot be due to the fact that coroutines 
 * (ie. IEnumerator WaitThenExecute()) cannot be static.
 * To get around this issue, I simply added this script to the
 * GameManager - so there's guaranteed to be 1 instance of this
 * class in every scene.
 */
#endregion README

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coroutines : MonoBehaviour
{
    //DEBUG 
    private float timeSinceBirth=0f;
    private static Coroutines instance;
    public delegate void voidDlg();

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    //DEBUG
    private void Update() {
        timeSinceBirth += Time.deltaTime;
        //Debug.Log(timeSinceBirth);
    }
    
    public void WaitThenExecute(float t, voidDlg dlg) {

        StartCoroutine(CorotWaitThenExecute(t, dlg));
    }
    private IEnumerator CorotWaitThenExecute(float t, voidDlg dlg) {
        yield return new WaitForSeconds(t);
        dlg?.Invoke(); 
    }

    public void LoopExecuteInterval(float loops, float interval, voidDlg dlg) {
        StartCoroutine(CorotLoopExecuteInterval(loops, interval, dlg));
    }
    private IEnumerator CorotLoopExecuteInterval(float loops, float interval, voidDlg dlg) {
        for (int iii = 0; iii < loops; iii++) {
            dlg?.Invoke();
            yield return new WaitForSeconds(interval);
        }
    }

    public static Coroutines GetInstance() {return instance;}

}
