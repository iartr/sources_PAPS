import { useState } from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import React from 'react';

function ModalShownOnButtonClick(props) {
	const [show, setShow] = useState(false);

	const handleClose = () => setShow(false);
	const handleShow = () => setShow(true);

	let variant = props.variant ?? "primary";

	let body = React.cloneElement(props.body,
		{
			onClose: handleClose
		});

	return (
		<>
			<Button variant={variant} onClick={handleShow} block>
				{props.children}
			</Button>
			<Modal show={show} onHide={handleClose}>
				<Modal.Header closeButton>
					<Modal.Title>{props.title}</Modal.Title>
				</Modal.Header>
				<Modal.Body>
					{body}
				</Modal.Body>
			</Modal>
		</>
	);
}

export default ModalShownOnButtonClick;