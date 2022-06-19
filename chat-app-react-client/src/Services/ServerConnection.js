import * as signalR from '@microsoft/signalr';
import AuthService from './AuthService';
import React from 'react';

export class ServerConnection {
	constructor(serverUrl) {
		this.serverUrl = serverUrl;
    this.connected = false;
    this.onNewMessageReceived = (chatId, message) => {};
	}

  setOnNewMessageReceived(onNewMessageReceived) {
    this.onNewMessageReceived = onNewMessageReceived;
  }

  isConnected() {
    return this.connection && this.connection.connected;
  }

	async connect() {
		const accessToken = AuthService.getAccessToken();
    if (!accessToken) {
      console.log("Access token not found");
      return;
    }

    const options = {
      skipNegotiation: true,
      transport: signalR.HttpTransportType.WebSockets,
      accessTokenFactory: () => {
        return `Bearer ${accessToken}`;
      }
    }

    let connection  = new signalR.HubConnectionBuilder()
      .withUrl(`${this.serverUrl}/chatshub`, options)
      .build();

    connection.on("NotifyMessageReceivedAsync", (message) => { 
      console.log("Server received message and responded: " + message);
    });

    connection.on("NotifyNewMessagePostedAsync", (chatId, message) => { 
      console.log(`New message notification received: chatId = ${chatId}, message = ${JSON.stringify(message)}`);
      if (this.onNewMessageReceived)
        this.onNewMessageReceived(chatId, message);
    });

    connection.onreconnected(() => {
      console.log("onreconnected");
      this.connected = true;
    });

    connection.onclose(() => {
      console.log("onconnected");
      this.connected = false;
    });

    this.connection = connection;

    let connectionPromise = this.connection.start();
    return connectionPromise;
	}

  async getChats() {
    return await this.connection.invoke("GetChatsAsync");
  }

  async getChatMessages(chatId) {
    let userInfo = AuthService.getUserInfo();
    let messages = await this.connection.invoke("GetMessagesAsync", chatId);    

    return messages.map((message, _) => {
      return {
        type: 'text',
        position: message.senderEmail === userInfo.email ? 'right' : 'left',
        text: message.text,
        date: Date.parse(message.sentTimeUtc),
        title: message.senderName,
      }
    });
  }

  async getUserByEmail(email) {
    let result = await this.connection.invoke("GetUserByEmailAsync", email);
    console.log(result);
    return result;
  }

  async sendMessageByEmailAsync(email, messageText) {
    let messageDto = {
      text: messageText,
      sentTimeUtc: new Date(),
    }

    console.log(messageDto);

    this.connection.invoke("SendPersonalMessageAsync", email, messageDto);
  }

  async sendMessageToChatAsync(chatId, messageDto) {
    await this.connection.invoke("SendMessageToChatAsync", chatId, messageDto);
  }

  async createChatroomAsync(chatroomDto) {
    await this.connection.invoke("CreateChatroomAsync", chatroomDto);
  }

  async updateChatroomAsync(chatroomDto) {
    await this.connection.invoke("UpdateChatroomAsync", chatroomDto);
  }  
}

export const ServerConnectionContext = React.createContext(new ServerConnection(""));