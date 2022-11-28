# demo-stash

Repo consists of 2 projects

Plant mania
* this was 2-person project - majority of my contribution is in Assets\PlantStuff
* scripts include logic for 3 different plants
* enemy logic was disabled for convenience

Basics:
* mouse wheel to select tool
* when gun is selected - 'q' to select seed type
* press 'e' looking at water to refill watering can 

Vine:

![demo_vine_complete](https://user-images.githubusercontent.com/73705079/204369442-0915cd73-0583-4c2c-b0b2-269344588ecf.png)

* made using Dreamteck Splines spline creation tool
* in the game for decorative purposes
* recursive (branches out)

Palm:

![demo_palm_complete](https://user-images.githubusercontent.com/73705079/204369903-567fa1fa-22f8-49ca-92a3-ea3d2e31cdde.png)

* can be watered and grown
* when fully grown, will spawn coconuts
* coconuts give seeds (gun ammo) if you reach them

Flower:

![demo_flower_complete](https://user-images.githubusercontent.com/73705079/204370701-f198ce20-cb5f-448a-b878-a3286306c1a2.png)

* can be watered and grown
* when fully grown, player will receive points - different amount of points for different colours


"eko-sim" - the second project:

![image](https://user-images.githubusercontent.com/73705079/204373501-e59bdf30-0741-4fb1-b40d-3da5851b9381.png)

* very much work in progress but shows potential
* includes my attempt (could not find online) at implementing A* pathfinding algorithm on a spherical terrain
* here i took basic implementation of A* on a grid and adapted it to work on a spherical bumpy terrain
* planet consists of 6 plains (normalized cube)
* mesh vertices used as nodes for PF
* logic for transitioning between different plains is in place (except when seeker and target are on opposite sides of the planet)
* seeker will go around the terrain that's too high (when vertex is too far up from normalized surface)

issues:
* no real solution for movement implemented - move target in ecene view
* path finding breaks when target gets below ground
* other instabilities
