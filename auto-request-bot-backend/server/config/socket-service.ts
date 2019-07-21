import { Subject } from 'rxjs';

import * as WebSocket from 'ws';
import { config } from '../config/config';

export enum SocketOp {
    ACK = "ACK",
    PLAIN = "PLAIN",
    CHECK_IN_CREATED = "CHECK_IN_CREATED",
    CHECK_IN_STARTED = "CHECK_IN_STARTED",
    CHECK_IN_UPDATED = "CHECK_IN_UPDATED",
    CHECK_IN_SKIP = "CHECK_IN_SKIP",
    MEMBER_NEED_CHECK_IN_UPDATED = "MEMBER_NEED_CHECK_IN_UPDATED",
};

let webSocketServer: WebSocket.Server;
const onMessage = new Subject<WebSocket.Data>();
const onDisconnected = new Subject<{ code: number, reason: string }>();

const socketService = {
    init: function () {

        webSocketServer = new WebSocket.Server({
            port: parseInt(config.wsPort)
        });

        webSocketServer.on('connection', (ws) => {

            console.log('client is connected');
            ws.emit(SocketOp.ACK, {
                message: 'client is connected'
            });

            ws.on('open', () => {
                console.log("open");
            });
            ws.on('message', (data: WebSocket.Data) => {
                console.log("client received message.");
                onMessage.next(data);
            });
            ws.on('close', (code, reason) => {
                onDisconnected.next({
                    code,
                    reason
                });
            });
        });
    },
    broadcast: function (op, data) {
        if (!webSocketServer) {
            console.log("WebSocketServer not initialized, ignore.");
            return;
        }

        const messageBlock = JSON.stringify({
            op,
            data
        });

        console.log(`broadcast message to ${webSocketServer.clients && webSocketServer.clients.size} clients: ${messageBlock}`);

        webSocketServer.clients.forEach((client) => {
            if (client.readyState === WebSocket.OPEN) {
                client.send(messageBlock);
            }
        });
    },
    onEvent: onMessage,
    onDisconnect: onDisconnected,
};

export default socketService;
