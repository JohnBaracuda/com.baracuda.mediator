using Baracuda.Utilities;
using TMPro;

namespace Baracuda.Bedrock.Pooling
{
    public class TextPool : PoolAsset<TMP_Text>
    {
        protected sealed override void OnReleaseInstance(TMP_Text instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(Parent);
        }

        protected sealed override void OnGetInstance(TMP_Text instance)
        {
            instance.SetActive(true);
        }
    }
}