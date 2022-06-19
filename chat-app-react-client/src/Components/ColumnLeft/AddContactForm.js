import React from 'react';
import AuthService from '../../Services/AuthService';
import { ServerConnectionContext } from '../../Services/ServerConnection';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';

class AddContactForm extends React.Component {

	static contextType = ServerConnectionContext;

	constructor(props) {
		super(props);

		let userInfo = AuthService.getUserInfo();

		this.state = {
			loggedInUserInfo: userInfo,
			inputEmail: "",
			message: "",
		}

		this.handleEmailChange = this.handleEmailChange.bind(this);
		this.handleMessageChange = this.handleMessageChange.bind(this);
		this.handleSubmit = this.handleSubmit.bind(this);
	}

	render() {
		return (
		<div>
			<Form onSubmit={this.handleSubmit}>
				<Form.Group>
					<Form.Label>Электронная почта пользователя:</Form.Label>
					<Form.Control type="email" value={this.state.inputEmail} onChange={this.handleEmailChange}/>
				</Form.Group>
				<Form.Group>
					<Form.Label>Сообщение</Form.Label>
					<Form.Control type="text" value={this.state.message} onChange={this.handleMessageChange} />
				</Form.Group>
				<Button variant="primary" type="submit" 
					disabled={this.state.inputEmail === "" || this.state.inputEmail === this.state.loggedInUserInfo.email}
				>
					Submit
				</Button>
			</Form>
		</div>
		);
	}

	handleEmailChange(event) {
		this.setState(
		{
			inputEmail: event.target.value,
		});
	}

	handleMessageChange(event) {
		this.setState({
			message: event.target.value
		});
	}

	handleSubmit(event) {
		event.preventDefault();
		const serverConnection = this.context.serverConnection;
		serverConnection
			.getUserByEmail(this.state.inputEmail)
			.then((user) => {
				if (!user) {
					console.log(`Did not find user with email ${this.state.inputEmail}`);
				}
				else {
					console.log(`Found user by email ${this.state.inputEmail}: user = ${user}`);	
					if (this.props.onAddContact)
						this.props.onAddContact(user, this.state.message);
				}
				
				if (this.props.onClose)
					this.props.onClose();
			});
	}
}

export default AddContactForm;