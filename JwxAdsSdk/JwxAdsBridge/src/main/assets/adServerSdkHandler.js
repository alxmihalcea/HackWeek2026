(function () {
    const containerId = 'ad-container';
    const sdk = window.cnxAdsSdk;

    if (!sdk) {
        onWebviewError("Ad server SDK not found");
        return;
    }

    const adsManager = sdk.fetchAdsManager({
        // placementId: '380498a1-0463-446c-8c67-6cccc8b25541',
        placementId: 'c8ce2228-4f32-4f1b-970f-b9e76449452c',
        adContainerId: containerId,
        player: {
            playbackmethod: 2,
            plcmt: 1
        },
        volume: 1,
        adServer: {
            lineitems: [
                {
                    id: '84805da0-273b-414e-8c90-2a25b86e349e',
                    cpm: 0.01,
                    floorPrice: 0.01,
                    url: 'https://assets.connatix.com/Elements/6dce5bde-736d-4f6b-88b7-ece3af19c862/Vast_-_15_Seconds.xml?isGdpr=[GDPR]&gdprPayload=[GDPR_CONSENT]&consent=[CONSENT]'
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
    })

    adsManager.on('setupError', (error) => {
        console.error('Setup error:', error);
    });
    adsManager.on('adError', (error) => {
        console.error('Ad error:', error);
    });
    adsManager.on('adBreakStarted', (payload) => {
        console.log('Ad break started', payload);
    });
    adsManager.on('adBreakEnded', (payload) => {
        console.log('Ad break ended', payload);
    });

    adsManager.startAdBreak({
        timeout: 10_000,
        type: 'midroll',
    });
})()