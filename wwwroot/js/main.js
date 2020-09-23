'use strict';

class Map {
    map;
    marker;
    followButton;
    flightPathButton;
    flightPath;
    layerControl;

    constructor() {
        this.InitMap();
    }

    UpdatePosition(lat, lng, heading) {
        const newPos = new L.LatLng(lat, lng);

        this.marker.setLatLng(newPos);
        this.marker.setRotationAngle(heading);

        if (!this.followButton.button.classList.contains("disabled"))
            this.map.setView(newPos);
    }

    DrawFlightPath(from, to) {
        if (this.flightPathButton.button.classList.contains("disabled"))
            return;

        this.flightPath = L.Polyline.Arc(from, to, {
            vertices: 200,
            color: "#ea70d4"
        }).addTo(this.map);
    }

    async InitMap() {
        // Basemap layer
        const cartoDBlayer = L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
            subdomains: 'abcd',
            maxZoom: 19
        });

        const openStreetMap = L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 19,
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        });

        const topoMap = L.tileLayer('https://{s}.tile.opentopomap.org/{z}/{x}/{y}.png', {
            maxZoom: 17,
            attribution: 'Map data: &copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors, <a href="http://viewfinderpanoramas.org">SRTM</a> | Map style: &copy; <a href="https://opentopomap.org">OpenTopoMap</a> (<a href="https://creativecommons.org/licenses/by-sa/3.0/">CC-BY-SA</a>)'
        });

        // NavAids layer
        const navAidsLayer = new L.TileLayer("https://{s}.tile.maps.openaip.net/geowebcache/service/tms/1.0.0/openaip_basemap@EPSG%3A900913@png/{z}/{x}/{y}.png", {
            maxZoom: 14,
            minZoom: 4,
            tms: true,
            subdomains: "12",
            format: "image/png",
            transparent: true,
            attribution: "<a href=\"https://www.openaip.net\" target=\"_blank\" style=\"\">openAIP</a>"
        });

        const baseMaps = {
            "OpenStreetMap": openStreetMap,
            "CartoDB Voyager": cartoDBlayer,
            "Topography": topoMap
        }

        this.map = L.map('map', {
            center: [0, 0],
            zoom: 13,
            layers: [openStreetMap],
            preferCanvas: true
        });

        this.map.on("dragstart", () => {
            this.followButton.disable();
        });

        const icon = L.icon({
            iconUrl: "./assets/airplane.svg",
            iconSize: [40, 40]
        });

        this.marker = L.marker([0,0], {
            icon: icon,
            rotationAngle: 0,
            rotationOrigin: 'center center'
        }).addTo(this.map);

        this.followButton = L.easyButton('<span class="material-icons">flight</span>', function () {
            const state = this.button.classList.contains("disabled");
            if (state)
                this.enable();
            else
                this.disable();
        }, 'Follow aircraft').addTo(this.map);

        this.flightPathButton = L.easyButton('<span class="material-icons">timeline</span>', () => {
            if (!this.flightPath)
                return;

            const state = this.flightPathButton.button.classList.contains("disabled");
            if (state) {
                this.flightPath.addTo(this.map);
                this.flightPathButton.enable();
            }
            else {
                this.flightPath.remove();
                this.flightPathButton.disable();
            }
        }, 'Display flight path').addTo(this.map);

        const overlayMaps = {
            "OpenAIP NavAids": navAidsLayer
        }

        this.layerControl = L.control.layers(baseMaps, overlayMaps).addTo(this.map);
    }
}

const app = new Vue({
    el: "#container",
    data: {
        map: null,
        acInfo: null,
        simConnected: null,
        barHidden: false,
        showSearch: false,
        searchText: "",
        searchResults: [],
        searchTimeout: null,
        db: null,
        selectedAirport: null
    },
    watch: {
        acInfo(newData, oldData) {
            this.map.UpdatePosition(newData.latitude, newData.longitude, newData.trueHeading);

            if (newData.gpsFlightPlanActive && (oldData === null || (newData.gpsWaypointIndex !== oldData.gpsWaypointIndex)))
                this.map.DrawFlightPath([newData.gpsNextWPLatitude, newData.gpsNextWPLongitude], [newData.gpsPrevWPLatitude, newData.gpsPrevWPLongitude]);
        }
    },
    async mounted() {
        const wsConnection = new signalR.HubConnectionBuilder().withUrl("/ws").withAutomaticReconnect().build();
        wsConnection.start();

        wsConnection.on("ReceiveData", (data) => {
            this.simConnected = data.isConnected;
            this.acInfo = data.data;
        });

        this.db = new Dexie("airports_database");
        this.db.version(1).stores({
            airports: '++id,country,name,icao,geolocation,radio,rwy,type'
        });

        const count = await this.db.airports.count();
        if (count === 0) {
            const response = await fetch("/get/airports");
            const result = await response.json();

            this.db.airports.bulkAdd(result.data);
        }

        this.map = new Map();

        const airportsLayer = L.layerGroup();
        this.db.airports.each(airport => {
            airportsLayer.addLayer(L.circleMarker([airport.geolocation.lat, airport.geolocation.lon], { radius: 2 }).on('click', () => {
                this.selectedAirport = airport;
            }));
        });
        this.map.layerControl.addOverlay(airportsLayer, "Airports");

        setTimeout(() => {
            if (this.simConnected === null)
                this.simConnected = false;
        }, 10000);
    },
    methods: {
        convertSecondsToHMS(value) {
            if (value < 0)
                return;

            return new Date(value * 1000).toISOString().substr(11, 8)
        },
        apDisplayName(value) {
            switch (value) {
                case "master":
                    return "AP";
                case "flightDirector":
                    return "FD";
                case "level":
                    return "LVL";
                case "altitude":
                    return "ALT";
                case "approach":
                    return "APR";
                case "backcourse":
                    return "BC";
                case "airspeed":
                    return "SPD";
                case "mach":
                    return "MCH";
                case "yawDamper":
                    return "YD";
                case "autothrottle":
                    return "AT";
                case "verticalHold":
                    return "VS";
                case "heading":
                    return "HDG";
                case "nav1":
                    return "NAV";
                default:
                    return "UKN";
            }
        },
        searchAirports() {
            clearTimeout(this.searchTimeout);

            if (this.searchText.trim().length === 0) {
                this.searchResults = [];
                return;
            }

            this.searchTimeout = setTimeout(async () => {
                this.searchResults = [];

                let results = await this.db.airports.where("icao").equalsIgnoreCase(this.searchText).toArray();
                if (results.length === 0) {
                    const regex = new RegExp(this.searchText, "i");
                    results = await app.db.airports.filter(airport => regex.test(airport.name)).toArray()
                }

                this.searchResults.push(results);
            }, 1000);
        },
        focusAirport(airport) {
            this.map.followButton.disable();
            this.showSearch = false;
            this.searchResults = [];
            this.searchText = "";
            this.map.map.setView([airport.geolocation.lat, airport.geolocation.lon], 13);
            this.selectedAirport = airport;
        }
    },
    computed: {
        fuelQuantityPercent() {
            return Math.round(this.acInfo.currentFuel / this.acInfo.totalFuel * 100);
        },
        autopilotProperties() {
            let temp = JSON.parse(JSON.stringify(this.acInfo.autopilot));
            delete temp.available;
            return temp;
        }
    }
});