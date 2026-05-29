# Villager System
- Villagers are economic units that can be played on gathering posts
- Villager cards are not shuffled into the player's deck
	- Instead, a single villager card is drawn each turn, and destroyed if it is not played during that turn.
- At the start of the game, 3 `Settler` cards are added the to player's starting hand.
- `Settlers` are special villager cards that:
	- Are not discarded at the end of each turn
	- Do not cost resources to play

```
The intention is that settlers are used to kickstart the player's economy, they can freely place the starting settlers into any resources they wish in order to execute their opening strategy.
```
# Resource Gathering
- Players start the game with a fixed amount of resources
- There is a resource gathering step at the beginning of each turn
- Resources are gathered from `Villagers` placed at gathering post buildings
	- `Town Centers`
	- `Mills (Hunting Cabins?)`
	- `Lumber Camps`
	- `Mining Camps`
- During the resource gathering step, resources are gathered from neighboring tiles of these buildings
	- Resources are only gathered if the player has villagers garrisoned in a gathering post
	- There are diminishing returns from having more villagers than resources around one gathering post
- After the resource gathering step, players must wait until their next turn to gather more resources
```
Players must strategically plan future turns by calculating what they'll be able to afford based on their current villager distribution
```

# Research System
#### Technologies
- There is a card type Called `Technology`
- Technology cards are played on a tile, and have various effects that allows you to expand your civilization.
	- Example: `Agriculture`
		- Unlocks  `Farms`, builds a free `Farm` on the target tile, and shuffles a few `Farm` cards into your deck for further expansion.
	- Example: `Double Bit Axe`
		- Play on a `Lumber Camp` to increase the amount of wood gathering by assigned villagers
	- Example: `Military Training`
		- Unlocks Barracks, Archery Ranges, Stables, and their relevant units, which are then shuffled into your deck.
# Town Progression
- Players can build structures by playing building cards
- There is a town progression system that unlocks new buildings when others are built.
	- Example:
		- Building a `Mill` and a `Lumber Camp` unlocks `(2) Mining Camps` and shuffles them into your deck.
## Action Cards
- Action cards can be played on units to perform a specific action
- e.g. `Torch` action - unit does additional damage against buildings this turn
- e.g. `Brace` action - spearmen units deal bonus damage to mounted units this turn
- e.g. `Shield` action - armored units take less damage next turn
## Zone of Influence
- Players have a zone of influence
	- Players can only build inside their zone of influence
- zone of influence increases when advancing to the next age
	- zone of influence should increase to take up half the map at its maximum level

## Landmarks
- Civilization specific buildings that give unique bonuses