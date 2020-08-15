using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;
using UnityEngine;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    public class StatefulEmptyClipProvider
    {
        private readonly ClipGenerator _clipGenerator;
        private AnimationClip _selectedEmptyClip;

        public StatefulEmptyClipProvider(ClipGenerator clipGenerator)
        {
            _clipGenerator = clipGenerator;
        }

        public AnimationClip Get()
        {
            if (_selectedEmptyClip == null) { 
                _selectedEmptyClip = _clipGenerator.GetOrCreateEmptyClip();
            }

            return _selectedEmptyClip;
        }
    }
}