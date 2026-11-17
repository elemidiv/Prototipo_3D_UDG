using UnityEngine;
using UnityEngine.UI;
namespace HelixJump
{
 public sealed class UIManager : MonoBehaviour
 {
  [SerializeField] Text currentScoreText,bestScoreText,currentLevelText,nextLevelText;
  [SerializeField] Slider progressSlider;
  [SerializeField,Min(0)] float progressSmoothing=10f;
  BallController ball;HelixController helix;
  public float ProgressTarget {get;private set;}
  public Slider ProgressSlider=>progressSlider;
  public void Configure(Text score,Text best){currentScoreText=score;bestScoreText=best;RefreshNow();}
  public void ConfigureProgress(Slider slider,Text current,Text next){progressSlider=slider;currentLevelText=current;nextLevelText=next;ResetProgress();}
  public static float NormalizeProgress(float top,float goal,float y){return Mathf.Approximately(top,goal)?0:Mathf.Clamp01((top-y)/(top-goal));}
  public void ResetProgress(){ProgressTarget=0;if(progressSlider!=null)progressSlider.SetValueWithoutNotify(0);}
  void Update()
  {
   RefreshNow();
   if(ball==null)ball=FindFirstObjectByType<BallController>();
   if(helix==null)helix=FindFirstObjectByType<HelixController>();
   if(progressSlider==null||ball==null||helix==null||helix.Top==null||helix.Goal==null)return;
   ProgressTarget=NormalizeProgress(helix.Top.position.y,helix.Goal.position.y,ball.transform.position.y);
   progressSlider.SetValueWithoutNotify(Mathf.Lerp(progressSlider.value,ProgressTarget,1-Mathf.Exp(-progressSmoothing*Time.deltaTime)));
  }
  public void RefreshNow()
  {
   var manager=GameManager.Instance;if(manager==null)return;
   if(currentScoreText!=null)currentScoreText.text="Score: "+manager.CurrentScore;
   if(bestScoreText!=null)bestScoreText.text="Best: "+manager.BestScore;
   if(currentLevelText!=null)currentLevelText.text=(manager.CurrentLevel+1).ToString();
   if(nextLevelText!=null)nextLevelText.text=(manager.CurrentLevel+2).ToString();
  }
 }
}
