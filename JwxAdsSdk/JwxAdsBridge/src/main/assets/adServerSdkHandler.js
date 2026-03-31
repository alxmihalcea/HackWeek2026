(function () {
    const containerId = 'ad-container';
    const sdk = window.cnxAdsSdk;

    if (!sdk) {
        console.log('Ad server SDK not found');
        return;
    }

    const adsManager = sdk.fetchAdsManager({
        placementId: '380498a1-0463-446c-8c67-6cccc8b25541',
        adContainerId: containerId,
        player: {
            playbackmethod: 2 /*VideoPlaybackMethodEnum._AUTOPLAY_PAGE_LOAD_SOUND_OFF*/,
            plcmt: 1 /*VideoPlacement.IN_STREAM*/
        },
        volume: 0,
        adServer: {
            lineitems: [
                {
                    id: '84805da0-273b-414e-8c90-2a25b86e349e',
                    cpm: 0.01,
                    floorPrice: 0.01,
                    url: 'https://assets.connatix.com/Elements/0a34019a-f275-4aac-a280-55114dffd5e4/Vast_30s.xml'
                }
            ],
            blockConnatixDemand: false
        },
        content: {
            id: 'content-id',
            keywords: ['keyword1', 'keyword2'],
            language: 'en',
            len: 60,
            title: 'content-title',
            url: 'https://www.example.com'
        },
        _jwpcInternals: {
            strategyOutcomeId: 'StrategyOutcomeID'
        }
    });

    adsManager.on('setupCompleted', () => {
        adsManager.startAdBreak({
            timeout: 10_000,
            type: 'preroll'
        });
    });
})();