VirtualPad was developed during the start of the Covid-19 pandemic, as a method for playing contact-free arcade games. By loading a React-based controller webpage, one's phone browser can interface with an instance of Unity over the web, no wi-fi connection or app download required.

Note: this project is in an unfinished/early prototype state and may not work properly without fixing outdated dependencies and manually modifying the configuration of ports and URLs in the code, depending on your own machine's IP address data.

VirtualPad-Unity is meant to be used in conjunction with VirtualPad-React: https://github.com/amckendry2/VirtualPad-React

#### VirtualPad-React
Running "node server/server.js" will start a game manager server on port 4000, and running "npm start" will start up a React frontend on port 3000.

#### VirtualPad-Unity
If the Unity test project is imported correctly, there should be a UI for generating a code once the game is running.

#### Testing VirtualPad
Once the server, frontend controller, and Unity game instance are running, entering the generated game code into the field on the frontend should cause the server to register the Unity instance as a game sesssion, and to add the frontend controller as a player. Additional players should be able to join by inputting the code on their own controller pages, up to the maximum player limit set in the Unity game instance. The state of each player's controller should then be displayed in the Unity game instance.
