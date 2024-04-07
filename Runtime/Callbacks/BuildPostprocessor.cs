#if UNITY_EDITOR
namespace Baracuda.Mediator.Callbacks
{
    internal class BuildPostprocessor : UnityEditor.Build.IPostprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPostprocessBuild(UnityEditor.Build.Reporting.BuildReport report)
        {
            Gameloop.RaiseBuildReportPostprocessor(new BuildReportData(report));
        }
    }
}
#endif