using UnityEngine;
using UnityEngine.Events;

public class EaseTimer : MonoBehaviour {

  public AnimationCurve transition;

  public UnityEvent timerEnd = new UnityEvent ();
  public UnityEventFloat timerTick = new UnityEventFloat ();
  public UnityEvent timerStart = new UnityEvent ();

  bool running = false;
  float duration;
  float timeLeft;

  public void StartTimer (float durationInSeconds) {

    running = true;
    this.duration = durationInSeconds;
    timeLeft = duration;
    timerStart.Invoke();

  }

  void Update () {

    if (!running) return;

    timeLeft -= Time.deltaTime;

    if (timeLeft <= 0.0f) {

      timeLeft = 0.0f;

      TimerTick ();
      timerEnd.Invoke ();
      running = false;

    } else {

      TimerTick ();

    }

  }

  void TimerTick () {

    var value = transition.Evaluate ((duration - timeLeft) / duration);
    timerTick.Invoke (value);

  }

}