using Markdig.Syntax;
#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#else
using UnityEngine.Experimental.UIElements.StyleSheets;
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;
#endif

namespace ThunderKit.Markdown.ObjectRenderers
{
    using static Helpers.VisualElementFactory;
    using static Helpers.VisualElementUtility;
    using static Helpers.UnityPathUtility;
    public class ParagraphRenderer : UIElementObjectRenderer<ParagraphBlock>
    {
        protected override void Write(UIElementRenderer renderer, ParagraphBlock obj)
        {
            renderer.Push(GetClassedElement<VisualElement>("paragraph"));
            renderer.WriteLeafInline(obj);
            renderer.Pop();
        }
    }
}
