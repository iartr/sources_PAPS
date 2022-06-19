import "./Home.css";
import {
	ServerConnection,
  ServerConnectionContext,
} from "../Services/ServerConnection";
import React from "react";
import { Container, Row, Col } from "react-bootstrap";
import SidePanel from "./ColumnLeft/SidePanel";
import ChatWindow from "./ColumnRight/ChatWindow";

class Home extends React.Component {
  state = {};

  constructor(props) {
    super(props);

    const serverConnection = new ServerConnection("https://violet-ducks-invite-193-143-64-51.loca.lt");

    this.state = {
      serverConnection: serverConnection,
      chats: [],
      selectedChat: null
    };

    this.handleAddContact = this.handleAddContact.bind(this);
    this.handleChatroomCreated = this.handleChatroomCreated.bind(this);
    this.handleChatroomUpdated = this.handleChatroomUpdated.bind(this);
    this.handleOnChatClicked = this.handleOnChatClicked.bind(this);
    this.fetchChats = this.fetchChats.bind(this);
    this.handleNewMessageReceived = this.handleNewMessageReceived.bind(this);
  }

  componentDidMount() {
    let serverConnection = this.state.serverConnection;
    serverConnection.connect().then(() => {
      console.log("Connected!");
      serverConnection.setOnNewMessageReceived(this.handleNewMessageReceived);
      this.fetchChats();
    });
  }

  handleNewMessageReceived(chatId, message) {
    this.fetchChats();
    if (this.state.selectedChat.id === chatId) {
      let selectedChat = this.state.selectedChat;
      this.setState({selectedChat: null});
      this.setState({selectedChat: selectedChat});
    }
  }

  handleAddContact(user, message) {
    this.state.serverConnection
      .sendMessageByEmailAsync(user.email, message)
      .then(() => {
        console.log("Sent personal message!");
        this.fetchChats();
      })
      .catch((error) =>
        console.log(
          `Error sending personal message, error = ${JSON.stringify(error)}`
        )
      );
  }

  handleChatroomCreated(newChat) {
    console.log(`handleCreateChat: ${newChat}`);
    this.fetchChats();
  }

  handleChatroomUpdated(chat) {
    console.log(`handleChatroomUpdated: ${chat.name}`);
    this.fetchChats();
  }

  handleOnChatClicked(chatItem) {
    let selectedChat = this.state.chats.find(chat => chat.id === chatItem.id);
    this.setState({selectedChat: selectedChat});
  }

  fetchChats() {
    let serverConnection = this.state.serverConnection;
    serverConnection.getChats().then((chats) => {
      console.log("Received chats!");
      let selectedChat = this.state.chat;
      if (selectedChat && chats.every(c => c.id !== selectedChat.id))
        selectedChat = null;
      
      this.setState({
        chat: selectedChat,
        chats: chats 
      });
    });
  }

  render() {
    const serverConnectionContextValue = {
      serverConnection: this.state.serverConnection,
    };

    return (
      <ServerConnectionContext.Provider value={serverConnectionContextValue}>
        <Container fluid className="main-container">
          <Row className="row-height-100">
            <Col lg={3}>
							<SidePanel 
                selectedChatId={this.state.selectedChat?.id}
								chats={this.state.chats}
								onChatClicked={this.handleOnChatClicked}
                onAddContact={this.handleAddContact}
                onNewChatroomCreated={this.handleChatroomCreated}
                className="sidepanel"
							/>
						</Col>
            <Col lg={9} className="d-flex">
              <ChatWindow
                className="chat-window"
                chat={this.state.selectedChat}
                onChatroomUpdated={this.handleChatroomUpdated}
              >
              </ChatWindow>
            </Col>
          </Row>
        </Container>
      </ServerConnectionContext.Provider>
    );
  }
}

export default Home;
