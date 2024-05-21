import "./style.css";
import javascriptLogo from "./javascript.svg";
import viteLogo from "/vite.svg";
import { setupCounter } from "./counter.js";
import { HubConnectionBuilder } from "@microsoft/signalr";

document.querySelector("#app").innerHTML = `
  <div>
    <a href="https://vitejs.dev" target="_blank">
      <img src="${viteLogo}" class="logo" alt="Vite logo" />
    </a>
    <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript" target="_blank">
      <img src="${javascriptLogo}" class="logo vanilla" alt="JavaScript logo" />
    </a>
    <h1>Hello Vite!</h1>
    <div class="card">
      <button id="counter" type="button"></button>
    </div>
    <p class="read-the-docs">
      Click on the Vite logo to learn more
    </p>
  </div>
`;

setupCounter(document.querySelector("#counter"));

let connection = new HubConnectionBuilder()
  .withUrl("http://localhost:5555/jobHub")
  .build();

connection.on("ReceiveMessage", (name, message) => {
  console.log(name, message);
});
connection.on("NotifyClient", (message) => {
  console.log(message);
});

connection
  .start()
  .then(() => connection.invoke("SendMessage", "Mario", "Hello"));

window.signalR = {
  connection: connection,
};
