import React, { useEffect, useState } from 'react';
import { NavLink, useParams } from 'react-router-dom';
import {Typography, Divider, Button, Card, CardContent, CardActions, Grid, TextField} from '@mui/material';
import { styled } from '@mui/system';

const StyledCard = styled(Card)({
  display: 'flex',
  flexDirection: 'column',
  height: '100%', borderRadius: theme => theme.spacing(1),
  boxShadow: '0 4px 8px rgba(0,0,0,0.1)',
  transition: 'transform 0.3s', '&:hover': {
      transform: 'scale(1.05)',
  },
});

const StyledCardImage = styled('img')({
  height: 200,
  objectFit: 'cover',
  borderTopLeftRadius: theme => theme.spacing(1),
  borderTopRightRadius: theme => theme.spacing(1),
});

const StyledCardContent = styled(CardContent)({
  flexGrow: 1,
});

const StyledCardActions = styled(CardActions)({
  justifyContent: 'space-between',
});

const Putovanje = () => {
  const { id } = useParams();
  const [putovanja, setPutovanja] = useState([]);
  const [putovanjeZaIzmenu, setPutovanjeZaIzmenu] = useState(null);
  const [rezervacijaAktivna, setRezervacijaAktivna] = useState(false);
  const [podaciRezervacije, setPodaciRezervacije] = useState({
    Ime: '',
    Prezime: '',
    BrojTelefona: '',
    Adresa: '',
    Grad: '',
    Email: '',
    BrojOsoba: 0
  });

  useEffect(() => {
    async function fetchPutovanja() {
      try {
        const response = await fetch(
          `https://localhost:7193/Putovanje/PreuzmiPutovanjaAgencije/${id}`
        );
        if (response.ok) {
          const data = await response.json();

          if (Array.isArray(data)) {
            setPutovanja(data);
          } else {
            console.error('Greška: Neispravan format podataka sa servera.');
          }
        } else {
          console.error('Greška prilikom preuzimanja putovanja. Status:', response.status);
        }
      } catch (error) {
        console.error('Greška prilikom preuzimanja putovanja:', error);
      }
    }
    fetchPutovanja();
  }, [id]);
  

const [izmenaPodataka, setIzmenaPodataka] = useState(false);
const [izmenjeniPodaci, setIzmenjeniPodaci] = useState({
  slika: '',
  mesto: '',
  brojNocenja: 0,
  cena: 0,
  prevoz: ''
});

const [dodavanjeAktivno, setDodavanjeAktivno] = useState(false);
const [noviPodaci, setNoviPodaci] = useState({
  slika: '',
  mesto: '',
  brojNocenja: 0,
  cena: 0,
  prevoz: ''
});

const handleRezervacijaPutovanja = async (putovanjeId) => {
  setRezervacijaAktivna(true);
  const izabranoPutovanje = putovanja.find((putovanje) => putovanje.id === putovanjeId);
  setPutovanjeZaIzmenu(izabranoPutovanje);
};

const handleRezervacija = async () => {
  try {
    const response = await fetch(`https://localhost:7193/Rezervacija/DodajRezervacijuPutovanja/${putovanjeZaIzmenu.id}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(podaciRezervacije),
    });

    if (response.ok) {
      console.log('Rezervacija je uspešno dodata.');
      setRezervacijaAktivna(false); 
      const updatedPutovanja = await response.json();
      setPutovanja(updatedPutovanja);
    } else {
      throw new Error('Greška prilikom dodavanja rezervacije.');
    }
  } catch (error) {
    console.error('Greška prilikom slanja zahtjeva za rezervaciju:', error);
  }
};

const handleDodajPutovanje = async () => {
  try {
    const response = await fetch(`https://localhost:7193/Putovanje/DodajPutovanjeAgenciji/${id}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(noviPodaci),
    });
    if (response.ok) {
      console.log('Novo putovanje je dodato.');
      const newData = await response.json();
      setPutovanja([...putovanja, newData]);
      setDodavanjeAktivno(false);
      setNoviPodaci({});
    } else {
      throw new Error('Greška prilikom dodavanja putovanja.');
    }
  } catch (error) {
    console.error('Greška prilikom slanja zahteva za dodavanje putovanja:', error);
  }
};

const handleSacuvajIzmene = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(
        `https://localhost:7193/Putovanje/AzurirajPutovanje/${izmenjeniPodaci.id}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(izmenjeniPodaci),
        }
      );
      if (response.ok) {
        console.log(`Putovanje sa ID ${izmenjeniPodaci.id} je ažurirano.`);
        setPutovanja((prevPutovanja) =>
          prevPutovanja.map((putovanje) =>
            putovanje.id === izmenjeniPodaci.id ? izmenjeniPodaci : putovanje
          )
        );
        setPutovanjeZaIzmenu(null);
        setIzmenaPodataka(false);
      } else {
        throw new Error('Neuspelo ažuriranje putovanja.');
      }
    } catch (error) {
      console.error('Greška prilikom ažuriranja putovanja:', error);
    }
  };
  
  const handleIzmeniPutovanje = (putovanjeId) => {
    const izabranoPutovanje = putovanja.find((putovanje) => putovanje.id === putovanjeId);
    setPutovanjeZaIzmenu(izabranoPutovanje);
    setIzmenjeniPodaci(izabranoPutovanje);
    setIzmenaPodataka(true);
  };
  
  const handleObrisiPutovanje = async (putovanjeId) => {
    try {
      const response = await fetch(
        `https://localhost:7193/Putovanje/ObrisiPutovanje/${putovanjeId}`,
        {
          method: 'DELETE',
        }
      );
      if (response.ok) {
        console.log(`Putovanje sa ID ${putovanjeId} je obrisano.`);
        setPutovanja(putovanja.filter((putovanje) => putovanje.id !== putovanjeId));
      } else {
        throw new Error('Neuspelo brisanje putovanja.');
      }
    } catch (error) {
      console.error('Greška prilikom brisanja putovanja:', error);
    }
  };

  return (
    <div style={{ textAlign: 'center', fontFamily: 'Roboto, sans-serif' }}>
      {putovanjeZaIzmenu && izmenaPodataka && (
      <div style={{ textAlign: 'center', marginTop: '20px' }}>
        <Typography variant="h6">Izmeni putovanje</Typography>
        <form onSubmit={handleSacuvajIzmene} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <TextField
            id="mestoIzmeni"
            label="Mesto"
            variant="outlined"
            value={izmenjeniPodaci.mesto}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, mesto: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="slikaIzmeni"
            label="URL slike"
            variant="outlined"
            value={izmenjeniPodaci.slika}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, slika: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="prevozIzmeni"
            label="Prevoz"
            variant="outlined"
            value={izmenjeniPodaci.prevoz}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, prevoz: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="brojNocenjaIzmeni"
            label="Broj noćenja"
            variant="outlined"
            type="number"
            value={izmenjeniPodaci.brojNocenja}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, brojNocenja: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="cenaIzmeni"
            label="Cena"
            variant="outlined"
            type="number"
            value={izmenjeniPodaci.cena}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, cena: e.target.value })
            }
            style={{ marginBottom: '20px', width: '300px' }}
          />
          <Button id="sacuvajIzmene" type="submit" variant="contained" sx={{ backgroundColor: '#900C3F'}}>
            Sačuvaj izmene
          </Button>
        </form>
      </div>
    )}
    <Divider><Button
      id="dodajPutovanje"
      variant="contained"
      sx={{ backgroundColor: '#900C3F'}}
      onClick={() => setDodavanjeAktivno(true)}
      style={{ marginTop: '20px' }}
    >
      Dodaj putovanje
    </Button></Divider>

    {putovanjeZaIzmenu && rezervacijaAktivna && (
      <div style={{ textAlign: 'center', marginTop: '20px' }}>
        <Typography variant="h6">Rezervacija putovanja</Typography>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleRezervacija();
          }}
          style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}
        >
          <TextField
            id="ime"
            label="Ime"
            variant="outlined"
            value={podaciRezervacije.Ime}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, Ime: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="prezime"
            label="Prezime"
            variant="outlined"
            value={podaciRezervacije.Prezime}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, Prezime: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="adresa"
            label="Adresa"
            variant="outlined"
            value={podaciRezervacije.Adresa}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, Adresa: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="grad"
            label="Grad"
            variant="outlined"
            value={podaciRezervacije.Grad}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, Grad: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="brojTelefona"
            label="Broj telefona"
            variant="outlined"
            value={podaciRezervacije.BrojTelefona}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, BrojTelefona: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="email"
            label="Email"
            variant="outlined"
            value={podaciRezervacije.Email}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, Email: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="brojOsoba"
            label="Broj osoba"
            variant="outlined"
            value={podaciRezervacije.BrojOsoba}
            onChange={(e) =>
              setPodaciRezervacije({ ...podaciRezervacije, BrojOsoba: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />

          <Button id="potvrdiRezervaciju" type="submit" variant="contained" sx={{ backgroundColor: '#900C3F'}}>
            Potvrdi rezervaciju
          </Button>
        </form>
      </div>
    )}
     {dodavanjeAktivno && (
      <div style={{ textAlign: 'center', marginTop: '20px' }}>
        <Typography variant="h6">Dodaj putovanje</Typography>
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleDodajPutovanje();
          }}
          style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}
        >
          <TextField
            id="mesto"
            label="Mesto"
            variant="outlined"
            value={noviPodaci.mesto}
            onChange={(e) =>
              setNoviPodaci({ ...noviPodaci, mesto: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="slika"
            label="URL slike"
            variant="outlined"
            value={noviPodaci.slika}
            onChange={(e) =>
              setNoviPodaci({ ...noviPodaci, slika: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="brojNocenja"
            label="Broj nocenja"
            variant="outlined"
            value={noviPodaci.brojNocenja}
            onChange={(e) =>
              setNoviPodaci({ ...noviPodaci, brojNocenja: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="cena"
            label="Cena"
            variant="outlined"
            value={noviPodaci.cena}
            onChange={(e) =>
              setNoviPodaci({ ...noviPodaci, cena: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            id="prevoz"
            label="Prevoz"
            variant="outlined"
            value={noviPodaci.prevoz}
            onChange={(e) =>
              setNoviPodaci({ ...noviPodaci, prevoz: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <Button id="dodaj" type="submit" variant="contained" sx={{ backgroundColor: '#900C3F'}}>
            Dodaj putovanje
          </Button>
        </form>
      </div>
    )}
      <Typography variant="h6" gutterBottom style={{ fontFamily: 'sans-serif' }}>
        <Divider style={{ marginTop: '30px' }}>Lista putovanja</Divider>
      </Typography>
      <Grid container spacing={3}>
            {putovanja.map((putovanje) => (
                <Grid item key={putovanje.id} xs={12} sm={6} md={4}>
                    <StyledCard id="listaPutovanja">
                        <StyledCardImage src={putovanje.slika} alt={putovanje.mesto}/>
                        <StyledCardContent>
                        <Typography variant="h6" component="div" sx={{ display: 'flex', justifyContent: 'space-between', paddingRight: '16px' }}>
                            {putovanje.mesto}
                            <NavLink to={`/Putovanje/${putovanje.id}/Recenzije`}>
                              <Button variant="outlined" color="secondary">
                                Recenzije
                              </Button>
                            </NavLink>
                            </Typography>
                            <Typography variant="body2" color="textSecondary" component="p">
                                Broj Nocenja: {putovanje.brojNocenja}
                            </Typography>
                            <Typography variant="body2" color="textSecondary" component="p">
                                Prevoz: {putovanje.prevoz}
                            </Typography>
                            <Typography variant="body2" color="textSecondary" component="p">
                                Cena: {putovanje.cena}
                            </Typography>
                            <Divider style={{ margin: '10px 0' }} />
                            <NavLink to={`/Putovanje/${putovanje.id}`}>
                              <Button variant="contained" sx={{ backgroundColor: '#900C3F'}}>
                                Pogledaj ponudu
                              </Button>
                            </NavLink>
                            <Divider style={{ margin: '10px 0' }} />
                            <Button
                              id="obrisi"
                              variant="outlined"
                              color="error"
                              onClick={() => handleObrisiPutovanje(putovanje.id)}
                            >
                              Obriši
                            </Button>
                            <Button
                              id="izmeni"
                              variant="outlined"
                              color="success"
                              onClick={() => handleIzmeniPutovanje(putovanje.id)}
                              style={{ marginLeft: '10px' }}
                            >
                              Izmeni
                            </Button>
                            <Button
                              id="rezervisi"
                              variant="outlined"
                              color="primary"
                              onClick={() => handleRezervacijaPutovanja(putovanje.id)}
                              style={{ marginLeft: '10px' }}
                            >
                              Rezerviši
                            </Button>
                        </StyledCardContent>
                    </StyledCard>
                </Grid>
            ))}
        </Grid>
    </div>
  );
};

export default Putovanje;