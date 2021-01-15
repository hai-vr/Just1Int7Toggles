using System.Linq;
using Hai.Just1Int7Toggles.Scripts.Editor.Internal.Reused;

namespace Hai.Just1Int7Toggles.Scripts.Editor.Internal
{
    internal class CasterCreator
    {
        private readonly AnimatorGenerator _animatorGenerator;
        private readonly TogglesManifest _manifest;

        public CasterCreator(AnimatorGenerator animatorGenerator, TogglesManifest manifest)
        {
            _animatorGenerator = animatorGenerator;
            _manifest = manifest;
        }

        public void CreateOrReplaceCaster()
        {
            var machinist = _animatorGenerator.CreateOrRemakeLayerAtSameIndex("Hai_J1I7T_Caster", 1f)
                .WithEntryPosition(0, -3)
                .WithExitPosition(0, -5);
            var idle = machinist.NewState("Idle", 0, -2);

            var uniqueParams = _manifest.Groups
                .Select(group => group.parameterName)
                .Distinct()
                .ToList();

            for (var index = 0; index < uniqueParams.Count; index++)
            {
                var uniqueParam = uniqueParams[index];

                var boolParam = new BoolParameterist(uniqueParam);
                var floatParam = new FloatParameterist(uniqueParam + "_F");
                _animatorGenerator.CreateParamsAsNeeded(boolParam, floatParam);

                var off = machinist.NewState(uniqueParam + " OFF", 2, index)
                    .Drives(floatParam, 0f)
                    .AutomaticallyMovesTo(idle);
                var on = machinist.NewState(uniqueParam + " ON", 3, index)
                    .Drives(floatParam, 1f)
                    .AutomaticallyMovesTo(idle);

                idle.TransitionsTo(on)
                    .When(boolParam).IsTrue()
                    .And(floatParam).IsLesserThan(0.5f);
                idle.TransitionsTo(off)
                    .When(boolParam).IsFalse()
                    .And(floatParam).IsGreaterThan(0.5f);
            }
        }
    }
}
