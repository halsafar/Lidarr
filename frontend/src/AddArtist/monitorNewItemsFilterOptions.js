import translate from 'Utilities/String/translate';

const monitorNewItemsFilterOptions = [
  { key: 'studio', value: 1, get label() {
    return translate('Studio');
  } },
  { key: 'single', value: 2, get label() {
    return translate('Single');
  } },
  { key: 'ep', value: 4, get label() {
    return translate('EP');
  } },
  { key: 'live', value: 8, get label() {
    return translate('Live');
  } },
  { key: 'compilation', value: 16, get label() {
    return translate('Compilation');
  } }
];

export default monitorNewItemsFilterOptions;
