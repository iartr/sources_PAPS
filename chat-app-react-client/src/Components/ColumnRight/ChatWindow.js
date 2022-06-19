import './ChatWindow.css';
import React from "react";
import { MessageList } from 'react-chat-elements';
import { ServerConnectionContext } from "../../Services/ServerConnection";
import { Input } from 'react-chat-elements';
import { Button as SendButton } from 'react-chat-elements';
import Button from 'react-bootstrap/Button';
import ChatWindowHeader from './ChatWindowHeader';
import Col from 'react-bootstrap/Col';
import Row from 'react-bootstrap/Row';
import Container from 'react-bootstrap/Container';
import ModalShownOnButtonClick from '../Common/ModalShownOnButtonClick';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import ChatDetails from './ChatDetails';
import { CHAT_TYPE_CHATROOM } from '../../Constants';

class ChatWindow extends React.Component {

	constructor(props) {
		super(props);

		this.state = {
			messages: [],
			selectedChatId: null,
			inputMessage: ""
		}

		this.fetchMessages = this.fetchMessages.bind(this);
		this.handleOnChange = this.handleOnChange.bind(this);
		this.handleSendMessage = this.handleSendMessage.bind(this);

		this.inputMessage = React.createRef();
	}

	componentDidUpdate(prevProps) {
		if (this.props.chat && this.props.chat.id !== prevProps.chat?.id) {
			console.log(this.props.chat);
			this.fetchMessages();
		}
	}

	render() {
    if (!this.props.chat)
      return <div>Не выбран чат</div>;

		return ( 
			<div className="chat-window-container">
        <Container fluid className="d-flex flex-column">
          <Row className="chat-window-header">
            <Col lg={9} className="chat-title">
              <ChatWindowHeader 
                chat={this.props.chat}/>
            </Col>
						<Col lg={3}>
							<ButtonGroup>
								{
									(this.props.chat.chatType !== CHAT_TYPE_CHATROOM)
										? (
											<></>
										)
										: (
											<ModalShownOnButtonClick 
												variant="light"
												title={"Информация о чате"}
												body={
													<ChatDetails 
														chat={this.props.chat}
														onChatroomUpdated={this.props.onChatroomUpdated}/>
												}
											>
												Инфо
											</ModalShownOnButtonClick>
										)
								}
							</ButtonGroup>
            </Col>
          </Row>
          <Row className="message-list-container">
            <Col className="w-100">
              <MessageList 
                toBottomHeight={'100%'}
                dataSource={this.state.messages}
              />
            </Col>
          </Row>
          <Row className="message-input-container">
            <Input
              ref={this.inputMessage}
              placeholder="New message..."
              onChange={this.handleOnChange}
              rightButtons={
                <SendButton
                  color="white"
                  backgroundColor="black"
                  text="Send"
                  onClick={this.handleSendMessage}
                  />}
              />
          </Row>
        </Container>
			</div>
		 );
	}

	handleOnChange(event) {
		this.setState({inputMessage: event.target.value});
	}

	handleSendMessage() {
		console.log(`sendMessage: ${this.state.inputMessage}`);
		const serverConnection = this.context.serverConnection;

		let message = {
			text: this.state.inputMessage,
			sentTimeUtc: new Date(),
		}

		let selectedChatId = this.props.chat.id;
		serverConnection.sendMessageToChatAsync(selectedChatId, message)
			.then(_ => {
				console.log(`ChatWindow: Sent message ${this.state.inputMessage} to chat ${selectedChatId}`);
				this.setState({inputMessage: ""});
				this.inputMessage.current.clear();
				this.fetchMessages();
			})
			.catch(error => console.log(`ChatWindow: Error while sending message to chat: ${error}`));
	}

	fetchMessages() {
		if (!this.props.chat)
			return;

		let selectedChatId = this.props.chat.id;
		
		const serverConnection = this.context.serverConnection;
		serverConnection.getChatMessages(selectedChatId)
			.then(messages => {
				console.log(`ChatWindow: Received messages, count = ${messages.length}`);
				this.setState({messages: messages});
			})
			.catch(error => console.log(`ChatWindow: Error while requesting chat messages: ${error}`));
	}
}

ChatWindow.contextType = ServerConnectionContext;
 
export default ChatWindow;