import React, { useCallback } from 'react';
import monitorNewItemsFilterOptions from 'AddArtist/monitorNewItemsFilterOptions';
import EnhancedSelectInput from './EnhancedSelectInput';

interface MonitorNewItemsFilterSelectInputProps {
  name: string;
  value: number;
  onChange(payload: object): void;
}

const allFlagsValue = monitorNewItemsFilterOptions.reduce(
  (acc, o) => acc + o.value,
  0
);

function MonitorNewItemsFilterSelectInput(
  props: MonitorNewItemsFilterSelectInputProps
) {
  const { value, onChange } = props;

  const selectedValues =
    value === 0
      ? monitorNewItemsFilterOptions.map((o) => o.value)
      : monitorNewItemsFilterOptions.reduce(
          (acc: number[], { value: optionValue }) => {
            // eslint-disable-next-line no-bitwise
            if ((value & optionValue) === optionValue) {
              acc.push(optionValue);
            }
            return acc;
          },
          []
        );

  const values = monitorNewItemsFilterOptions.map(({ value, label }) => ({
    key: value,
    value: label,
  }));

  const onChangeWrapper = useCallback(
    ({ name, value }: { name: string; value: number[] }) => {
      const filter = value.reduce((acc, v) => acc + v, 0);
      onChange({ name, value: filter === allFlagsValue ? 0 : filter });
    },
    [onChange]
  );

  return (
    <EnhancedSelectInput
      {...props}
      value={selectedValues}
      values={values}
      onChange={onChangeWrapper}
    />
  );
}

export default MonitorNewItemsFilterSelectInput;
