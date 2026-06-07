import moment from 'moment';
import PropTypes from 'prop-types';
import React, { Component } from 'react';
import Button from 'Components/Link/Button';
import SpinnerButton from 'Components/Link/SpinnerButton';
import Modal from 'Components/Modal/Modal';
import ModalBody from 'Components/Modal/ModalBody';
import ModalContent from 'Components/Modal/ModalContent';
import ModalFooter from 'Components/Modal/ModalFooter';
import ModalHeader from 'Components/Modal/ModalHeader';
import { kinds, sizes } from 'Helpers/Props';
import translate from 'Utilities/String/translate';

function formatInterval(minutes) {
  if (minutes === 0) {
    return translate('Disabled');
  }

  return moment.duration(minutes, 'minutes').humanize().replace(/an?(?=\s)/, '1');
}

class EditTaskIntervalModal extends Component {

  constructor(props, context) {
    super(props, context);

    this.state = {
      interval: props.interval
    };
  }

  componentDidUpdate(prevProps) {
    if (prevProps.isOpen !== this.props.isOpen && this.props.isOpen) {
      this.setState({ interval: this.props.interval });
    }
  }

  onIntervalChange = (event) => {
    const value = parseInt(event.target.value);

    this.setState({ interval: isNaN(value) ? 0 : Math.max(0, value) });
  };

  onResetPress = () => {
    this.setState({ interval: this.props.defaultInterval });
  };

  onSavePress = () => {
    this.props.onSave(this.state.interval);
  };

  render() {
    const {
      isOpen,
      name,
      defaultInterval,
      onModalClose
    } = this.props;

    const { interval } = this.state;

    return (
      <Modal
        isOpen={isOpen}
        size={sizes.SMALL}
        onModalClose={onModalClose}
      >
        <ModalContent onModalClose={onModalClose}>
          <ModalHeader>
            {translate('EditTaskInterval')} - {name}
          </ModalHeader>

          <ModalBody>
            <div>
              <label htmlFor="taskInterval">
                {translate('IntervalMinutes')}
              </label>

              <input
                id="taskInterval"
                type="number"
                min="0"
                value={interval}
                onChange={this.onIntervalChange}
                style={{ marginLeft: '10px', width: '100px' }}
              />

              <div style={{ marginTop: '8px', color: '#888', fontSize: '13px' }}>
                {formatInterval(interval)}
                {interval !== defaultInterval && (
                  <span>
                    {' '}&mdash; {translate('Default')}: {formatInterval(defaultInterval)} ({defaultInterval} {translate('Minutes')})
                  </span>
                )}
              </div>
            </div>
          </ModalBody>

          <ModalFooter>
            <Button
              kind={kinds.DEFAULT}
              onPress={this.onResetPress}
            >
              {translate('ResetToDefault')}
            </Button>

            <Button
              kind={kinds.DEFAULT}
              onPress={onModalClose}
            >
              {translate('Cancel')}
            </Button>

            <SpinnerButton
              kind={kinds.PRIMARY}
              isSpinning={false}
              onPress={this.onSavePress}
            >
              {translate('Save')}
            </SpinnerButton>
          </ModalFooter>
        </ModalContent>
      </Modal>
    );
  }
}

EditTaskIntervalModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  name: PropTypes.string.isRequired,
  interval: PropTypes.number.isRequired,
  defaultInterval: PropTypes.number.isRequired,
  onSave: PropTypes.func.isRequired,
  onModalClose: PropTypes.func.isRequired
};

export default EditTaskIntervalModal;
