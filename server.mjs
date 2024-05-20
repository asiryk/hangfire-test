import { createServer } from "node:http";

const PORT = process.env.PORT || 3000;

createServer(async (req, res) => {
    const response = `${req.method} ${req.url}`;
    console.log(response);
    res.end(response);
}).listen(PORT, () => console.log(`listening port ${PORT}`));
