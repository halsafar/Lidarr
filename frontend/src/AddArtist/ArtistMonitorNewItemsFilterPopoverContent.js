import React from 'react';
import DescriptionList from 'Components/DescriptionList/DescriptionList';
import DescriptionListItem from 'Components/DescriptionList/DescriptionListItem';
import translate from 'Utilities/String/translate';

function ArtistMonitorNewItemsFilterPopoverContent() {
  return (
    <DescriptionList>
      <DescriptionListItem
        title={translate('NoFilter')}
        data={translate('MonitorNewItemsFilterNoFilterData')}
      />
      <DescriptionListItem
        title={translate('Studio')}
        data={translate('MonitorNewItemsFilterStudioData')}
      />
      <DescriptionListItem
        title={translate('Single')}
        data={translate('MonitorNewItemsFilterSingleData')}
      />
      <DescriptionListItem
        title={translate('EP')}
        data={translate('MonitorNewItemsFilterEPData')}
      />
      <DescriptionListItem
        title={translate('Live')}
        data={translate('MonitorNewItemsFilterLiveData')}
      />
      <DescriptionListItem
        title={translate('Compilation')}
        data={translate('MonitorNewItemsFilterCompilationData')}
      />
    </DescriptionList>
  );
}

export default ArtistMonitorNewItemsFilterPopoverContent;
