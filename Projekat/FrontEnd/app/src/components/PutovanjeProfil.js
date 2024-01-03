import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
  Card,
  CardContent,
  Typography,
  CircularProgress,
  Grid,
  Divider,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  TextField,
} from '@mui/material';

const PutovanjeProfil = () => {
  const { id } = useParams();
  const [aktivnosti, setAktivnosti] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openForm, setOpenForm] = useState(false);
  const [novaAktivnost, setNovaAktivnost] = useState({ naziv: '', cena: '' });
  const [izmenaAktivnosti, setIzmenaAktivnosti] = useState(false);
  const [izmenjeniPodaci, setIzmenjeniPodaci] = useState({ id: '', naziv: '', cena: '' });

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = () => {
    setLoading(true);
    fetch(`https://localhost:7193/Aktivnosti/PreuzmiAktivnostiPutovanja/${id}`)
      .then((response) => response.json())
      .then((data) => {
        setAktivnosti(data);
        setLoading(false);
      })
      .catch((error) => {
        console.error('Greška pri preuzimanju aktivnosti:', error);
        setLoading(false);
      });
  };

  const handleDodajAktivnost = () => {
    setOpenForm(true);
  };

  const handleCloseForm = () => {
    setOpenForm(false);
  };

  const handleChange = (event) => {
    const { name, value } = event.target;
    setNovaAktivnost((prevAktivnost) => ({
      ...prevAktivnost,
      [name]: value,
    }));
  };

  const handleSubmit = () => {
    fetch(`https://localhost:7193/Aktivnosti/DodajAktivnostUPutovanje/${id}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(novaAktivnost),
    })
      .then(() => {
        setNovaAktivnost({ naziv: '', cena: '' });
        setOpenForm(false);
        fetchData();
      })
      .catch((error) => {
        console.error('Greška pri dodavanju aktivnosti:', error);
      });
  };

  const handleObrisiAktivnost = (aktivnostId) => {
    fetch(`https://localhost:7193/Aktivnosti/ObrisiAktivnost/${aktivnostId}`, {
      method: 'DELETE',
    })
      .then(() => {
        fetchData();
      })
      .catch((error) => {
        console.error('Greška pri brisanju aktivnosti:', error);
      });
  };

  const handleIzmeniAktivnost = (aktivnostId) => {
    const aktivnostZaIzmenu = aktivnosti.find((aktivnost) => aktivnost.id === aktivnostId);
    setIzmenjeniPodaci({
      id: aktivnostZaIzmenu.id,
      naziv: aktivnostZaIzmenu.naziv,
      cena: aktivnostZaIzmenu.cena,
    });
    setIzmenaAktivnosti(true);
  };

  const handleIzmenaChange = (event) => {
    const { name, value } = event.target;
    setIzmenjeniPodaci((prevPodaci) => ({
      ...prevPodaci,
      [name]: value,
    }));
  };

  const handleSacuvajIzmenu = () => {
    fetch(`https://localhost:7193/Aktivnosti/AzurirajAktivnost/${izmenjeniPodaci.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(izmenjeniPodaci),
    })
      .then(() => {
        setIzmenjeniPodaci({ id: '', naziv: '', cena: '' });
        setIzmenaAktivnosti(false);
        fetchData();
      })
      .catch((error) => {
        console.error('Greška pri ažuriranju aktivnosti:', error);
      });
  };

  return (
    <div style={{ padding: '20px' }}>
      <Typography variant="h6" gutterBottom style={{ fontFamily: 'sans-serif' }}>
        <Divider variant='h4'>Aktivnosti na putovanju</Divider>
      </Typography>
      
      {loading ? (
        <CircularProgress style={{ marginTop: '20px' }} />
      ) : (
        <Grid container spacing={3} style={{ marginTop: '20px' }}>
          {aktivnosti.map((aktivnost) => (
            <Grid item xs={12} sm={6} md={4} key={aktivnost.id}>
              <Card variant="outlined">
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {aktivnost.naziv}
                  </Typography>
                  <Typography variant="body1" color="textSecondary">
                    Cena: {aktivnost.cena}
                  </Typography>
                  <Button
                    variant="outlined"
                    color="secondary"
                    onClick={() => handleObrisiAktivnost(aktivnost.id)}
                    style={{ marginTop: '10px' }}
                  >
                    Obriši
                  </Button>
                  <Button
                    variant="outlined"
                    color="primary"
                    onClick={() => handleIzmeniAktivnost(aktivnost.id)}
                    style={{ marginLeft: '10px', marginTop: '10px' }}
                  >
                    Izmeni
                  </Button>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}
      <Button
        variant="outlined" 
        style={{ 
          borderColor: 'purple',
          margin: '0 auto', 
          display: 'block', 
          marginTop: '20px', 
        }}
        onClick={handleDodajAktivnost}
      >
        Dodaj
      </Button>

      <Dialog open={openForm} onClose={handleCloseForm}>
        <DialogTitle>Dodaj novu aktivnost</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="naziv"
            name="naziv"
            label="Naziv"
            type="text"
            fullWidth
            value={novaAktivnost.naziv}
            onChange={handleChange}
          />
          <TextField
            margin="dense"
            id="cena"
            name="cena"
            label="Cena"
            type="text"
            fullWidth
            value={novaAktivnost.cena}
            onChange={handleChange}
          />
          <Button variant="contained" color="primary" onClick={handleSubmit}>
            Sačuvaj
          </Button>
        </DialogContent>
      </Dialog>
      <Dialog open={izmenaAktivnosti} onClose={() => setIzmenaAktivnosti(false)}>
        <DialogTitle>Izmeni aktivnost</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="naziv"
            name="naziv"
            label="Naziv"
            type="text"
            fullWidth
            value={izmenjeniPodaci.naziv}
            onChange={handleIzmenaChange}
          />
          <TextField
            margin="dense"
            id="cena"
            name="cena"
            label="Cena"
            type="text"
            fullWidth
            value={izmenjeniPodaci.cena}
            onChange={handleIzmenaChange}
          />
          <Button variant="contained" color="primary" onClick={handleSacuvajIzmenu}>
            Sačuvaj izmene
          </Button>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default PutovanjeProfil;
