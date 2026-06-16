window.radarMap = {
    map: null,
    markersLayer: null,
    dotNetRef: null,

    init: function (dotNetRef) {
        if (this.map) {
            return;
        }

        this.dotNetRef = dotNetRef;

        this.map = L.map('radar-map').setView([42.7, 25.5], 8);

        var osmBase = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(this.map);

        var railInfra = L.tileLayer('https://{s}.tiles.openrailwaymap.org/standard/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openrailwaymap.org/">OpenRailwayMap</a>'
        }).addTo(this.map);

        var railSpeed = L.tileLayer('https://{s}.tiles.openrailwaymap.org/maxspeed/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openrailwaymap.org/">OpenRailwayMap</a>'
        });

        var overlays = {
            'Инфраструктура': railInfra,
            'Ограничение': railSpeed
        };

        L.control.layers(null, overlays).addTo(this.map);

        this.markersLayer = L.layerGroup().addTo(this.map);
    },

    updateTrains: function (trains) {
        if (!this.map || !this.markersLayer) {
            return;
        }

        this.markersLayer.clearLayers();
        this.markers = {};

        trains.forEach(function (train) {
            var iconFile = 'images/trainIcoBlue.png';
            var shadowColor = '#1a3a5a';
            switch (train.category) {
                case 1: // MBV
                    iconFile = 'images/trainIcoYellow.png';
                    shadowColor = '#5a4a1a';
                    break;
                case 3: // BV
                    iconFile = 'images/trainIcoRed.png';
                    shadowColor = '#5a1a1a';
                    break;
                case 4: // KPV
                    iconFile = 'images/trainIcoGreen.png';
                    shadowColor = '#1a5a2a';
                    break;
                case 5: // PV
                    iconFile = 'images/trainIcoBlue.png';
                    shadowColor = '#1a1a5a';
                    break;
            }

            var trainIcon = L.divIcon({
                className: 'train-marker',
                html: '<div style="position:relative;width:50px;height:53px;">' +
                          '<img src="' + iconFile + '" style="width:100%;height:100%;" />' +
                          '<span style="position:absolute;top:12px;left:0;right:0;display:flex;align-items:center;justify-content:center;font-size:8px;font-weight:900;line-height:1;color:#fff;text-shadow:0 0 2px ' + shadowColor + ',0 0 2px ' + shadowColor + ';white-space:nowrap;pointer-events:none;">' + train.trainNumber + '</span>' +
                      '</div>',
                iconSize: [50, 53],
                iconAnchor: [25, 53],
                popupAnchor: [0, -70]
            });

            var marker = L.marker([train.latitude, train.longitude], { icon: trainIcon });

            // Color scheme
            var colors = { bg: '#2980b9', border: '#1a5276', light: '#d6eaf8', lightAlt: '#c2dff2' };
            switch (train.category) {
                case 1: colors = { bg: '#d4a017', border: '#7d6608', light: '#fef9e7', lightAlt: '#fdf0c4' }; break;
                case 3: colors = { bg: '#c0392b', border: '#78281f', light: '#fadbd8', lightAlt: '#f5b7b1' }; break;
                case 4: colors = { bg: '#27ae60', border: '#1a5a2a', light: '#d5f5e3', lightAlt: '#b8ecd1' }; break;
                case 5: colors = { bg: '#2980b9', border: '#1a5276', light: '#d6eaf8', lightAlt: '#c2dff2' }; break;
            }

            var delayHtml = train.delayMinutes > 0
                ? '<span style="color:#c0392b;font-weight:700;">' + train.delayMinutes + ' мин</span>'
                : '<span style="color:#27ae60;font-weight:600;">Навреме</span>';

            var progressHtml = '';
            if (train.totalStops > 0) {
                progressHtml =
                    '<div style="margin-top:8px;">' +
                        '<div style="display:flex;justify-content:space-between;font-size:11px;margin-bottom:3px;">' +
                            '<span>Прогрес</span>' +
                            '<span>' + train.passedStops + '/' + train.totalStops + ' спирки (' + train.progressPercent + '%)</span>' +
                        '</div>' +
                        '<div style="background:#e9ecef;border-radius:6px;height:8px;overflow:hidden;">' +
                            '<div class="progress-bar-animated" style="width:' + train.progressPercent + '%;height:100%;background-color:' + colors.bg + ';border-radius:6px;"></div>' +
                        '</div>' +
                    '</div>';
            }

            var timetableRowsHtml = '';
            var currentRowId = '';
            if (train.timetableStops && train.timetableStops.length > 0) {
                train.timetableStops.forEach(function (stop, idx) {
                    var rowId = '';
                    var rowBg = (idx % 2 === 0) ? colors.light : colors.lightAlt;
                    var nameStyle = 'font-size:11px;padding:4px 8px;';
                    var timeStyle = 'font-size:11px;padding:4px 8px;white-space:nowrap;';
                    var rowStyle = 'background:' + rowBg + ';';

                    if (stop.isCurrent) {
                        rowId = 'cur-' + train.trainNumber;
                        currentRowId = rowId;
                        rowStyle = 'background:' + colors.bg + ';';
                        nameStyle += 'font-weight:700;color:#fff;';
                        timeStyle += 'color:#fff;font-weight:700;';
                    } else if (stop.isPassed) {
                        nameStyle += 'opacity:0.45;';
                        timeStyle += 'opacity:0.45;';
                    }

                    var timeCell = stop.arrivalTime || stop.departureTime
                        ? (stop.arrivalTime || '') + (stop.arrivalTime && stop.departureTime ? ' / ' : '') + (stop.departureTime || '')
                        : '—';

                    timetableRowsHtml +=
                        '<tr id="' + rowId + '" style="' + rowStyle + '">' +
                            '<td style="' + nameStyle + '">' + stop.stationName + '</td>' +
                            '<td style="' + timeStyle + '">' + timeCell + '</td>' +
                        '</tr>';
                });
            }

            var timetableHtml = timetableRowsHtml
                ? '<div id="timetable-' + train.trainNumber + '" style="' +
                      'display:none;flex:1;min-height:0;overflow-y:auto;' +
                      'border-left:3px solid ' + colors.bg + ';' +
                      'border-right:3px solid ' + colors.bg + ';' +
                      'scrollbar-width:thin;scrollbar-color:' + colors.bg + ' ' + colors.light + ';' +
                  '">' +
                      '<table style="width:100%;border-collapse:collapse;">' +
                          '<thead style="position:sticky;top:0;z-index:1;">' +
                              '<tr style="background:' + colors.bg + ';">' +
                                  '<th style="font-size:10px;padding:4px 8px;text-align:left;color:rgba(255,255,255,0.9);font-weight:600;">Гара</th>' +
                                  '<th style="font-size:10px;padding:4px 8px;text-align:left;color:rgba(255,255,255,0.9);font-weight:600;">Пристигане / Заминаване</th>' +
                              '</tr>' +
                          '</thead>' +
                          '<tbody>' + timetableRowsHtml + '</tbody>' +
                      '</table>' +
                  '</div>'
                : '';

            var razpisanieBtn = timetableRowsHtml
                ? '<button onclick="(function(btn){' +
                      'var t=document.getElementById(\'timetable-' + train.trainNumber + '\');' +
                      'var body=document.getElementById(\'popupbody-' + train.trainNumber + '\');' +
                      'if(t.style.display===\'none\'){' +
                          't.style.display=\'flex\';' +
                          't.style.flexDirection=\'column\';' +
                          'body.style.display=\'none\';' +
                          'btn.innerHTML=\'▲ Разписание\';' +
                          (currentRowId
                              ? 'setTimeout(function(){var r=document.getElementById(\'' + currentRowId + '\');if(r){r.scrollIntoView({block:\'center\'});}},50);'
                              : '') +
                      '}else{' +
                          't.style.display=\'none\';' +
                          'body.style.display=\'block\';' +
                          'btn.innerHTML=\'▼ Разписание\';' +
                      '}' +
                  '})(this)" style="' +
                      'display:block;width:100%;padding:7px;font-size:11px;font-weight:700;' +
                      'background:' + colors.bg + ';border:none;cursor:pointer;color:#fff;' +
                      'letter-spacing:0.5px;border-radius:0 0 8px 8px;flex-shrink:0;' +
                  '">▼ Разписание</button>'
                : '';

            var popupHtml =
                '<div style="min-width:240px;font-family:inherit;display:flex;flex-direction:column;max-height:340px;">' +
                    '<div style="background:' + colors.bg + ';color:#fff;padding:8px 24px 8px 12px;border-radius:8px 8px 0 0;display:flex;align-items:center;flex-shrink:0;">' +
                        '<span style="background:rgba(255,255,255,0.25);padding:2px 8px;border-radius:4px;font-size:11px;">' + train.categoryName + '</span>' +
                    '</div>' +
                    '<div id="popupbody-' + train.trainNumber + '" style="padding:10px 12px;font-size:12px;background:' + colors.light + ';border:1px solid ' + colors.border + ';border-top:none;overflow-y:auto;">' +
                        (train.startStationName || train.finalStationName ?
                            '<div style="margin-bottom:6px;"><b>Маршрут:</b> ' + (train.startStationName || '—') + ' → ' + (train.finalStationName || '—') + '</div>' : '') +
                        (train.stationName ?
                            '<div style="margin-bottom:4px;"><b>Текуща гара:</b> ' + train.stationName +
                                (train.currentStationArrivalTime ? ' <span style="color:#6c757d;font-size:11px;">(' + train.currentStationArrivalTime + ')</span>' : '') +
                            '</div>' : '') +
                        (train.nextStationName ?
                            '<div style="margin-bottom:4px;"><b>Следваща гара:</b> ' + train.nextStationName +
                                (train.nextStationDepartureTime ? ' <span style="color:#6c757d;font-size:11px;">(' + train.nextStationDepartureTime + ')</span>' : '') +
                            '</div>' : '') +
                        '<div style="margin-bottom:4px;"><b>Закъснение:</b> ' + delayHtml + '</div>' +
                        (train.wagonCount > 0 ?
                            '<div style="margin-bottom:4px;"><b>Вагони:</b> ' + train.wagonCount + '</div>' : '') +
                        (train.lastReportedAt ?
                            '<div style="margin-bottom:4px;color:#6c757d;font-size:11px;">Последно обновяване: ' + train.lastReportedAt + '</div>' : '') +
                        progressHtml +
                    '</div>' +
                    timetableHtml +
                    razpisanieBtn +
                '</div>';

            marker.bindPopup(popupHtml, { className: 'train-popup-leaflet', maxWidth: 320, minWidth: 240 });

            var self = this;
            marker.on('popupopen', function () {
                if (self.activeMarker && self.activeMarker !== marker) {
                    var prevEl = self.activeMarker.getElement();
                    if (prevEl) {
                        prevEl.style.transform = prevEl.style.transform.replace(' scale(1.4)', '');
                        prevEl.style.zIndex = '';
                    }
                }
                self.activeMarker = marker;
                var el = marker.getElement();
                if (el) {
                    if (el.style.transform.indexOf('scale(1.4)') === -1) {
                        el.style.transform += ' scale(1.4)';
                    }
                    el.style.transformOrigin = 'center bottom';
                    el.style.zIndex = '10000';
                }
                if (self.dotNetRef) {
                    self.dotNetRef.invokeMethodAsync('OnTrainPopupOpened', train.trainNumber);
                }
            });

            marker.on('popupclose', function () {
                var el = marker.getElement();
                if (el) {
                    el.style.transform = el.style.transform.replace(' scale(1.4)', '');
                    el.style.zIndex = '';
                }
                if (self.activeMarker === marker) {
                    self.activeMarker = null;
                }
                if (self.dotNetRef) {
                    self.dotNetRef.invokeMethodAsync('OnTrainPopupClosed');
                }
            });

            this.markersLayer.addLayer(marker);
            this.markers[train.trainNumber] = marker;
        }.bind(this));
    },

    openTrainPopup: function (trainNumber) {
        var marker = this.markers[trainNumber];
        if (marker && this.map) {
            this.map.closePopup();

            this.map.once('moveend', function () {
                marker.openPopup();
            });

            this.map.flyTo(marker.getLatLng(), 14, { duration: 1.2 });
        }
    }
};
