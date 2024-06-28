using System.Collections.Generic;
using MedievalConquerors.Engine.Actions;
using MedievalConquerors.Engine.Core;
using MedievalConquerors.Engine.Data;
using MedievalConquerors.Engine.Logging;
using MedievalConquerors.Extensions;

namespace MedievalConquerors.Engine.GameComponents;

// TODO: Unit tests for this component
public class CardSystem : GameComponent, IAwake
{
    private readonly List<Card> _playable = new();

    private Match _match;
    private ILogger _logger;
    private TargetSystem _targetSystem;

    public void Awake()
    {
        _logger = Game.GetComponent<ILogger>();
        _match = Game.GetComponent<Match>();
        _targetSystem = Game.GetComponent<TargetSystem>();
    }

    public bool IsPlayable(Card card) => _playable.Contains(card);

    public void Refresh()
    {
        _playable.Clear();
        
        foreach (var card in _match.CurrentPlayer.Hand)
        {
            var targetCandidates = _targetSystem.GetTargetCandidates(card);
            if (targetCandidates.Count == 0)
                continue;
            
            var randomTargetTile = targetCandidates.GetRandom();
            var playAction = new PlayCardAction(card, randomTargetTile);
            var validatorResult = playAction.Validate(Game);
            
            if (validatorResult.IsValid)
                _playable.Add(card);
        }
    }
}