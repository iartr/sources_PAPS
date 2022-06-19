import './ChatWindowHeader.css';
import Alert from 'react-bootstrap/Alert';

function ChatWindowHeader(props) {
  return (
    <Alert className="chat-name">
      <div>{props.chat.name}</div>
    </Alert>
  );
}

export default ChatWindowHeader;