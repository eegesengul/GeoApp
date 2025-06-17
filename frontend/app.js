let drawnPolygon = null;

const map = L.map('map').setView([39.92, 32.85], 7); // Türkiye merkezli

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
  attribution: '&copy; OpenStreetMap katkıcıları'
}).addTo(map);

const drawnItems = new L.FeatureGroup();
map.addLayer(drawnItems);

const drawControl = new L.Control.Draw({
  draw: {
    polyline: false,
    rectangle: false,
    circle: false,
    marker: false,
    circlemarker: false,
    polygon: {
      allowIntersection: false,
      showArea: true,
      drawError: {
        color: 'red',
        message: 'Geçersiz alan!'
      }
    }
  },
  edit: {
    featureGroup: drawnItems,
    edit: true,
    remove: false
  }
});

map.addControl(drawControl);

map.on('draw:created', function (e) {
  if (window.editingMode) return;

  drawnItems.clearLayers();
  drawnPolygon = e.layer;
  drawnItems.addLayer(drawnPolygon);

  drawnPolygon.bindPopup(`
    <b>Yeni Alan</b><br>
    <button onclick="removeCurrentDrawing()">🗑 Vazgeç</button>
  `).openPopup();
});

function savePolygon() {
  if (!drawnPolygon) {
    alert("Lütfen önce bir alan çizin.");
    return;
  }

  const geojson = drawnPolygon.toGeoJSON().geometry;
  const wkt = wellknown.stringify(geojson);

  const body = {
    name: "Haritadan Alan",
    description: "Leaflet ile çizildi.",
    wktGeometry: wkt
  };

  fetch("https://localhost:7262/api/areas", {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(body)
  })
  .then(response => {
    if (response.ok) {
      alert("Alan başarıyla kaydedildi!");
      location.reload();
    } else {
      alert("Hata oluştu.");
    }
  });
}

function loadSavedAreas() {
  fetch("https://localhost:7262/api/areas")
    .then(res => res.json())
    .then(data => {
      const geoJsonLayer = L.geoJSON(data, {
        onEachFeature: function (feature, layer) {
          layer.addTo(drawnItems);
          layer.bindPopup(
            `<b>${feature.properties.name}</b><br>${feature.properties.description}<br>
            <button onclick="deleteArea('${feature.properties.id}')">🗑 Sil</button>
            <button onclick="editArea('${feature.properties.id}', '${feature.properties.name}', '${feature.properties.description}')">✏️ Düzenle</button>`
          );

          layer.on('click', function () {
            drawnPolygon = layer;
          });
        }
      });

      map.on(L.Draw.Event.EDITED, function (e) {
        e.layers.eachLayer(layer => {
          drawnPolygon = layer;
        });
      });
    });
}

loadSavedAreas();

function deleteArea(id) {
  if (!confirm("Bu alanı silmek istediğinize emin misiniz?")) return;

  fetch(`https://localhost:7262/api/areas/${id}`, {
    method: "DELETE"
  })
  .then(response => {
    if (response.status === 204) {
      alert("Alan silindi.");
      location.reload();
    } else {
      alert("Silme işlemi başarısız.");
    }
  })
  .catch(err => {
    console.error("Silme hatası:", err);
    alert("Sunucu hatası.");
  });
}

function editArea(id, name, description) {
  const newName = prompt("Yeni ad:", name);
  if (newName === null) return;

  const newDesc = prompt("Yeni açıklama:", description);
  if (newDesc === null) return;

  drawnItems.clearLayers();

  fetch(`https://localhost:7262/api/areas`)
    .then(res => res.json())
    .then(geoJson => {
      const feature = geoJson.features.find(f => f.properties.id === id);
      if (!feature) {
        alert("Alan bulunamadı.");
        return;
      }

      const layer = L.geoJSON(feature).getLayers()[0];
      drawnItems.addLayer(layer);
      drawnPolygon = layer;

      if (!confirm("Yeni geometriyi değiştirmek istiyor musun? Değiştirmiyorsan doğrudan onayla.")) {
        return;
      }

      const geojson = drawnPolygon.toGeoJSON().geometry;
      const wkt = wellknown.stringify(geojson);

      const body = {
        name: newName,
        description: newDesc,
        wktGeometry: wkt
      };

      fetch(`https://localhost:7262/api/areas/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(body)
      }).then(response => {
        if (response.ok) {
          alert("Alan güncellendi.");
          location.reload();
        } else {
          alert("Güncelleme başarısız.");
        }
      });
    });
}

// Yeni çizimi kaldırma fonksiyonu
function removeCurrentDrawing() {
  if (drawnPolygon) {
    drawnItems.removeLayer(drawnPolygon);
    drawnPolygon = null;
  }
   location.reload();
}
