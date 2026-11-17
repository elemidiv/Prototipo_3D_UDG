using UnityEngine;
namespace HelixJump
{
 [DisallowMultipleComponent]
 public sealed class ChristmasBallEffects : MonoBehaviour
 {
  [SerializeField] ParticleSystem bounceSnow;
  [SerializeField] ParticleSystem superConfetti;
  TrailRenderer trail; Gradient normalColors; float normalWidth, normalTime;
  bool captured;
  public int BounceBursts {get;private set;}
  public int SuperBursts {get;private set;}
  public ParticleSystem BounceParticles=>bounceSnow;
  public ParticleSystem SuperParticles=>superConfetti;
  public void Configure(ParticleSystem snow,ParticleSystem confetti){bounceSnow=snow;superConfetti=confetti;}
  void Capture()
  {
   if(captured)return;
   trail=GetComponentInChildren<TrailRenderer>(true);
   if(trail!=null){normalColors=trail.colorGradient;normalWidth=trail.widthMultiplier;normalTime=trail.time;}
   captured=true;
  }
  public void Bounce(){if(bounceSnow!=null){bounceSnow.transform.position=transform.position+Vector3.down*0.13f;bounceSnow.Emit(14);}BounceBursts++;}
  public void ActivateSuper()
  {
   Capture();
   if(superConfetti!=null){superConfetti.transform.position=transform.position;superConfetti.Emit(32);}SuperBursts++;
   if(trail!=null){trail.widthMultiplier=0.3f;trail.time=0.4f;var g=new Gradient();g.SetKeys(new[]{new GradientColorKey(new Color(1,0.12f,0.16f),0),new GradientColorKey(new Color(1,0.8f,0.18f),0.45f),new GradientColorKey(new Color(0.05f,0.7f,0.24f),1)},new[]{new GradientAlphaKey(1,0),new GradientAlphaKey(0,1)});trail.colorGradient=g;}
  }
  public void EndSuper(){Capture();if(trail!=null){trail.widthMultiplier=normalWidth;trail.time=normalTime;trail.colorGradient=normalColors;}}
  public void ClearAll(){EndSuper();if(bounceSnow!=null)bounceSnow.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);if(superConfetti!=null)superConfetti.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);if(trail!=null)trail.Clear();BounceBursts=0;SuperBursts=0;}
  void OnDisable(){ClearAll();}
 }
}
