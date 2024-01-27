import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import {Card, CardContent, Typography, CircularProgress, Grid, Divider, Button, Dialog, DialogTitle, DialogContent, TextField} from '@mui/material';

const PutovanjeProfil = () => {
  const { id } = useParams();
  const [aktivnosti, setAktivnosti] = useState([]);
  const [loading, setLoading] = useState(true);
  const [openForm, setOpenForm] = useState(false);
  const [novaAktivnost, setNovaAktivnost] = useState({ naziv: '', cena: '' });
  const [izmenaAktivnosti, setIzmenaAktivnosti] = useState(false);
  const [izmenjeniPodaci, setIzmenjeniPodaci] = useState({ id: '', naziv: '', cena: '' });
  const [reservations, setReservations] = useState([]);
  const [loadingReservations, setLoadingReservations] = useState(true);
  //rez
  const [novaRezervacija, setNovaRezervacija] = useState({ ime: '', prezime: '', adresa: '', grad: '', brojTelefona: '', brojOsoba: '', email: '' });
  const [izmenaRezervacije, setIzmenaRezervacije] = useState(false);
  const [izmenjeniPodaciR, setIzmenjeniPodaciR] = useState({ id: '', ime: '', prezime: '', adresa: '', grad: '', brojTelefona: '', brojOsoba: '', email: '' });


  const fetchReservations = () => {
    setLoadingReservations(true);
    fetch(`https://localhost:7193/Rezervacija/PreuzmiRezervacijePutovanja/${id}`)
      .then((response) => response.json())
      .then((data) => {
        setReservations(data);
        setLoadingReservations(false);
      })
      .catch((error) => {
        console.error('Greška pri preuzimanju rezervacija:', error);
        setLoadingReservations(false);
      });
  };

  useEffect(() => {
    fetchReservations();
  }, []);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = () => {
    setLoading(true);
    fetch(`https://localhost:7193/Aktivnosti/PreuzmiAktivnostiNaPutovanju/${id}`)
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

  const fetchDataR = () => {
    setLoading(true);
    fetch(`https://localhost:7193/Rezervacija/PreuzmiRezervacijePutovanja/${id}`)
      .then((response) => response.json())
      .then((data) => {
        setReservations(data);
        setLoading(false);
      })
      .catch((error) => {
        console.error('Greška pri preuzimanju rezervacije:', error);
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

  //za brisanje rezervacije

  const handleObrisiRezervaciju = (rezervacijaId) => {
    fetch(`https://localhost:7193/Rezervacija/ObrisiRezervaicju/${rezervacijaId}`, {
      method: 'DELETE',
    })
      .then(() => {
        fetchDataR();
      })
      .catch((error) => {
        console.error('Greška pri brisanju rezervacije:', error);
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

  const handleIzmeniRezervaciju = (rezervacijaId) => {
    const rezervacijaZaIzmenu = reservations.find((reservation) => reservation.id === rezervacijaId);
    setIzmenjeniPodaciR({
      id: rezervacijaZaIzmenu.id,
      ime: rezervacijaZaIzmenu.ime,
      prezime: rezervacijaZaIzmenu.prezime,
      adresa: rezervacijaZaIzmenu.adresa,
      grad: rezervacijaZaIzmenu.grad,
      brojTelefona: rezervacijaZaIzmenu.brojTelefona,
      brojOsoba: rezervacijaZaIzmenu.brojOsoba,
      email: rezervacijaZaIzmenu.email
    });
    setIzmenaRezervacije(true);
  };

  const handleIzmenaChangeR = (event) => {
    const { name, value } = event.target;
    setIzmenjeniPodaciR((prevPodaci) => ({
      ...prevPodaci,
      [name]: value,
    }));
  };

  const handleSacuvajIzmenuR = () => {
    fetch(`https://localhost:7193/Rezervacija/AzurirajRezervaciju/${izmenjeniPodaciR.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(izmenjeniPodaciR),
    })
      .then(() => {
        setIzmenjeniPodaciR({ id: '', ime: '', prezime: '', adresa: '', grad: '', brojTelefona: '', brojOsoba: '', email: '' });
        setIzmenaRezervacije(false);
        fetchDataR();
      })
      .catch((error) => {
        console.error('Greška pri ažuriranju rezervacije:', error);
      });
  };

  return (
    <div style={{ padding: '20px' }}>
      <Typography variant="h6" gutterBottom style={{ fontFamily: 'sans-serif' }}>
        <Divider variant='h4'>Aktivnosti na putovanju</Divider>
        <Button
        id="dodajAktivnost"
        variant="contained"
        sx={{backgroundColor: '#900C3F'}}
        style={{
          margin: '0 auto', 
          display: 'block'
        }}
        onClick={handleDodajAktivnost}
      >
        Dodaj
      </Button>
      </Typography>
      
      {loading ? (
        <CircularProgress style={{ marginTop: '20px' }} />
      ) : (
        <Grid container spacing={3} style={{ marginTop: '20px' }}>
          {aktivnosti.map((aktivnost) => (
            <Grid item xs={12} sm={6} md={4} key={aktivnost.id}>
              <Card id="aktivnosti" variant="outlined">
                <CardContent>
                  <Typography variant="h6" gutterBottom>
                    {aktivnost.naziv}
                  </Typography>
                  <Typography variant="body1" color="textSecondary">
                    Cena: {aktivnost.cena}
                  </Typography>
                  <Button
                    id="obrisiAktivnost"
                    variant="outlined"
                    color="error"
                    onClick={() => handleObrisiAktivnost(aktivnost.id)}
                    style={{ marginTop: '10px' }}
                  >
                    Obriši
                  </Button>
                  <Button
                    id="izmeniAktivnost"
                    variant="outlined"
                    color="success"
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

      <Typography variant="h6" gutterBottom style={{ fontFamily: 'sans-serif', marginTop: '40px' }}>
      <Divider variant='h4'>Rezervacije za putovanje</Divider>
    </Typography>
    {loadingReservations ? (
      <CircularProgress style={{ marginTop: '20px' }} />
    ) : (
      <Grid container spacing={3} style={{ marginTop: '20px' }}>
        {reservations.map((rezervacija) => (
          <Grid item xs={12} sm={6} md={4} key={rezervacija.id}>
            <Card id="rezervacije" variant="outlined">
              <CardContent>
                <Typography variant="body2" gutterBottom>
                  Ime i prezime: {rezervacija.ime} {rezervacija.prezime} 
                </Typography>
                <Typography variant="body2" gutterBottom>
                  Adresa: {rezervacija.adresa}, {rezervacija.grad} 
                </Typography>
                <Typography variant="body2" gutterBottom>
                  Kontakt: {rezervacija.brojTelefona}, {rezervacija.email} 
                </Typography>
                <Typography variant="body2" gutterBottom>
                  Broj Osoba: {rezervacija.brojOsoba}
                </Typography>
                <Button
                    id="obrisiRezervaciju"
                    variant="outlined"
                    color="error"
                    onClick={() => handleObrisiRezervaciju(rezervacija.id)}
                    style={{ marginTop: '10px' }}
                  >
                    Obriši
                  </Button>
                <Button
                    id="izmeniRezervaciju"
                    variant="outlined"
                    color="success"
                    onClick={() => handleIzmeniRezervaciju(rezervacija.id)}
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

      <Dialog open={openForm} onClose={handleCloseForm}>
        <DialogTitle>Dodaj novu aktivnost</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="nazivAktivnosti"
            name="naziv"
            label="Naziv"
            type="text"
            fullWidth
            value={novaAktivnost.naziv}
            onChange={handleChange}
          />
          <TextField
            margin="dense"
            id="cenaAktivnosti"
            name="cena"
            label="Cena"
            type="text"
            fullWidth
            value={novaAktivnost.cena}
            onChange={handleChange}
          />
          <Button id="sacuvajAktivnost" variant="contained" sx={{backgroundColor: '#900C3F'}} onClick={handleSubmit}>
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
            id="izmeniNazivAktivnosti"
            name="naziv"
            label="Naziv"
            type="text"
            fullWidth
            value={izmenjeniPodaci.naziv}
            onChange={handleIzmenaChange}
          />
          <TextField
            margin="dense"
            id="izmeniCenuAktivnosti"
            name="cena"
            label="Cena"
            type="text"
            fullWidth
            value={izmenjeniPodaci.cena}
            onChange={handleIzmenaChange}
          />
          <Button id="sacuvajIzmeneAktivnosti" variant="contained" sx={{backgroundColor: '#900C3F'}} onClick={handleSacuvajIzmenu}>
            Sačuvaj izmene
          </Button>
        </DialogContent>
      </Dialog>

      <Dialog open={izmenaRezervacije} onClose={() => setIzmenaRezervacije(false)}>
        <DialogTitle>Izmeni rezervacije</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            margin="dense"
            id="ime"
            name="ime"
            label="Ime"
            type="text"
            fullWidth
            value={izmenjeniPodaciR.ime}
            onChange={handleIzmenaChangeR}
          />
          <TextField
            margin="dense"
            id="prezime"
            name="prezime"
            label="Prezime"
            type="text"
            fullWidth
            value={izmenjeniPodaciR.prezime}
            onChange={handleIzmenaChangeR}
          />
          <TextField
            margin="dense"
            id="adresa"
            name="adresa"
            label="Adresa"
            type="text"
            fullWidth
            value={izmenjeniPodaciR.adresa}
            onChange={handleIzmenaChangeR}
          />
          <TextField
            margin="dense"
            id="grad"
            name="grad"
            label="Grad"
            type="text"
            fullWidth
            value={izmenjeniPodaciR.grad}
            onChange={handleIzmenaChangeR}
          />
          <TextField
            margin="dense"
            id="brojTelefona"
            name="brojTelefona"
            label="Broj telefona"
            type="text"
            fullWidth
            value={izmenjeniPodaciR.brojTelefona}
            onChange={handleIzmenaChangeR}
          />
          <TextField
            margin="dense"
            id="email"
            name="email"
            label="Email"
            type="text"
            fullWidth
            value={izmenjeniPodaciR.email}
            onChange={handleIzmenaChangeR}
          />
          <TextField
            margin="dense"
            id="brojOsoba"
            name="brojOsoba"
            label="Broj osoba"
            type="number"
            fullWidth
            value={izmenjeniPodaciR.brojOsoba}
            onChange={handleIzmenaChangeR}
          />

          <Button id="sacuvajIzmeneRezervacije" variant="contained" sx={{backgroundColor: '#900C3F'}} onClick={handleSacuvajIzmenuR}>
            Sačuvaj izmene
          </Button>
        </DialogContent>
      </Dialog>
    </div>
  );
};

export default PutovanjeProfil;
