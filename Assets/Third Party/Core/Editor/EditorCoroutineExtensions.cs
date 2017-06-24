using UnityEngine;
using System.Collections;
using UnityEditor;

public static class EditorCoroutineExtensions {

    #region EditorWindow
    public static void StartCoroutine(this EditorWindow thisRef, IEnumerator coroutine) {
        EditorCoroutine.StartCoroutine(coroutine, thisRef);
    }

    public static void StartCoroutine(this EditorWindow thisRef, string methodName) {
        EditorCoroutine.StartCoroutine(methodName, thisRef);
    }

    public static void StartCoroutine(this EditorWindow thisRef, string methodName, object value) {
        EditorCoroutine.StartCoroutine(methodName, value, thisRef);
    }

    public static void StopCoroutine(this EditorWindow thisRef, IEnumerator coroutine) {
        EditorCoroutine.StopCoroutine(coroutine, thisRef);
    }

    public static void StopCoroutine(this EditorWindow thisRef, string methodName) {
        EditorCoroutine.StopCoroutine(methodName, thisRef);
    }

    public static void StopAllCoroutines(this EditorWindow thisRef) {
        EditorCoroutine.StopAllCoroutines(thisRef);
    }
    #endregion


}
