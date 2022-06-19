import React from 'react';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import PersonPlus from '../../Assets/person-plus.svg';
import Col from 'react-bootstrap/Col';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import { ServerConnectionContext } from '../../Services/ServerConnection';
import AuthService from '../../Services/AuthService';


class ChatDetails extends React.Component {

	constructor(props) {
		super(props);

		if (this.props.chat) {
			let chat = this.props.chat;
			this.state = {
				chatName: chat.name,
				newMemberEmail: "",
				members: chat.members,
			}
		}
		else {
			this.state = {
				chatName: "",
				newMemberEmail: "",
				members: [],
			}
		}

		this.removeMember = this.removeMember.bind(this);
		this.addMember = this.addMember.bind(this);
		this.handleNewMemberEmailChange = this.handleNewMemberEmailChange.bind(this);
		this.handleChatNameChange = this.handleChatNameChange.bind(this);
		this.handleSubmit = this.handleSubmit.bind(this);
		this.createChatroom = this.createChatroom.bind(this);
		this.updateChatroom = this.updateChatroom.bind(this);
	}

	removeMember(memberId) {
		let members = this.state.members;
		let memberToRemove = members.find((member) => member.id === memberId);
		let index = members.indexOf(memberToRemove);
		console.log(index);
		members.splice(index, 1);
		this.setState({members: members});
	}

	addMember() {
		let email = this.state.newMemberEmail;
		let serverConnection = this.context.serverConnection;
		serverConnection.getUserByEmail(email)
			.then(user => {
				if (!user) {
					console.log("user was not found");
					return;
				}

				console.log("addMember: found user by email");
				let member = {
					id: user.id,
					name: user.name,
					email: user.email,
				}
				let members = this.state.members;
				members.push(member);
				this.setState({members: members});
			});
	}

	handleNewMemberEmailChange(event) {
		let newMemberEmail = event.target.value;
		this.setState({newMemberEmail: newMemberEmail});
	}

	handleChatNameChange(event) {
		let chatName = event.target.value;
		this.setState({chatName: chatName});
	}

	handleSubmit(event) {
		event.preventDefault();
		let newChatroomDto = {
			id: this.props.chat ? this.props.chat.id : null,
			name: this.state.chatName,
			membersIds: this.state.members.map(member => member.id),
		}

		if (this.props.chat) {
			this.updateChatroom(newChatroomDto);
		}
		else{
			this.createChatroom(newChatroomDto);
		}

		if (this.props.onClose) {
			this.props.onClose();
		}
	}

	updateChatroom(updatedChatroomDto) {
		this.context.serverConnection.updateChatroomAsync(updatedChatroomDto)
			.then(() => {
				console.log(`Updated chatroom with id ${updatedChatroomDto.id}`);
				if (this.props.onChatroomUpdated)
					this.props.onChatroomUpdated(this.props.chat);
			});
	}

	createChatroom(newChatroomDto) {
		this.context.serverConnection.createChatroomAsync(newChatroomDto)
			.then(() => {
				console.log("Created new chatroom");
				if (this.props.onNewChatroomCreated)
					this.props.onNewChatroomCreated();
			});
	}

	render() { 
		let currentUserInfo = AuthService.getUserInfo();
		let ownerEmail = this.props.chat?.owner?.email;
		let isCurrentUserOwner = !this.props.chat || ownerEmail === currentUserInfo.email;

		return (
			<Form>
				<Form.Group>
					<Form.Control 
						type="text" placeholder="Название чата" disabled={!isCurrentUserOwner}
						value={this.state.chatName} onChange={this.handleChatNameChange}
					/>
				</Form.Group>
				<Form.Group>
					<Form.Label>
						Участники
					</Form.Label>
					{
						isCurrentUserOwner
							? (
								<Form.Row>
									<Col lg={10}>
										<Form.Control 
											type="email" placeholder="Добавьте участника"
											value={this.state.newMemberEmail} onChange={this.handleNewMemberEmailChange}
										>
										</Form.Control>
									</Col>
									<Col lg={2}>
										<Button 
											className="button-right"
											onClick={this.addMember}
										>
											<img src={PersonPlus} />
										</Button>
									</Col>
								</Form.Row>
							)
							: (<div></div>)
					}
					
					{this.state.members.map((member, _) => {
						return (
						<ListGroupItem >
							{`${member.name} (${member.email})${(member.email === ownerEmail) ? " — Владелец" : ""}`}
							{
								isCurrentUserOwner && member.email !== currentUserInfo.email
									? <Button onClick={() => this.removeMember(member.id)}>
										Удалить
										</Button>
									: <div></div>
							}
						</ListGroupItem>);
					})}
				</Form.Group>
				{
					isCurrentUserOwner ?
					(	
						<Button type="submit" variant="primary" onClick={this.handleSubmit}>
							{this.props.chat ? "Сохранить" : "Создать"}
						</Button>
					)
					: (<div></div>)
				}
			</Form>
		);
	}
}

ChatDetails.contextType = ServerConnectionContext;
 
export default ChatDetails;