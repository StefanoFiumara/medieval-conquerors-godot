using System;
using System.Collections;
using MedievalConquerors.Engine.Core;

namespace MedievalConquerors.Engine.Actions
{
    public class ActionPhase
    {
        public GameAction Owner { get; }
        public Action<IGame> Handler { get; }

        public Func<IGame, GameAction, IEnumerator> Viewer { get; set; }

        public ActionPhase(GameAction owner, Action<IGame> handler)
        {
            Owner = owner;
            Handler = handler;
        }

        public IEnumerator Flow(IGame gameState)
        {
            bool hitKeyFrame = false;

            if (Viewer != null)
            {
                var sequence = Viewer(gameState, Owner);
                while (sequence.MoveNext())
                {
                    var isKeyFrame = (sequence.Current is true);

                    if (isKeyFrame)
                    {
                        hitKeyFrame = true;
                        Handler(gameState);
                    }

                    yield return null;
                }
            }

            if (!hitKeyFrame)
            {
                Handler(gameState);
            }
        }
    }
}