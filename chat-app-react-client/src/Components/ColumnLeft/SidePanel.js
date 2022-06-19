import { Component } from 'react';
import Row from 'react-bootstrap/Row';
import ModalShownOnButtonClick from '../Common/ModalShownOnButtonClick';
import AddContactForm from './AddContactForm';
import ChatDetails from '../ColumnRight/ChatDetails';
import Col from 'react-bootstrap/Col';
import PersonPlus from '../../Assets/person-plus.svg';
import CreateChatIcon from '../../Assets/chat-icon.svg';
import { ChatList } from 'react-chat-elements';
import ChatroomIcon from '../../Assets/people.svg';

class SidePanel extends Component {
  state = {};

  mapChatDtoToChatItem(chatDto) {
    let imageUrl = chatDto.imageUrl ?? ChatroomIcon;
    
    return { 
      id: chatDto.id,
      title: chatDto.name,
      subtitle: chatDto.lastMessage,
      date: Date.parse(chatDto.lastMessageTime),
      avatar: imageUrl
    };
  }

  render() {

    return (
      <div>
        <Row className="justify-space-between">
          <ModalShownOnButtonClick
            block
            title="Новый контакт"
            body={
              <AddContactForm
                onAddContact={this.props.onAddContact}
              ></AddContactForm>
            }
          >
            Добавить контакт
          </ModalShownOnButtonClick>

          <ModalShownOnButtonClick
            title="Новый чат"
            body={
              <ChatDetails
                onNewChatroomCreated={this.props.onNewChatroomCreated}
              ></ChatDetails>
            }
          >
						Новый чат
          </ModalShownOnButtonClick>
        </Row>
        <Row>
          <Col>
            <ChatList 
              onClick={this.props.onChatClicked}
              dataSource={this.props.chats.map(this.mapChatDtoToChatItem)}
            />
          </Col>
        </Row>
      </div>
    );
  }
}

export default SidePanel;
