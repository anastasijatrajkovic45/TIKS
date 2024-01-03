import React, { useEffect, useState } from 'react';
import { NavLink, useParams } from 'react-router-dom';
import {
  Typography,
  Divider,
  Button,
  Card,
  CardContent,
  Grid,
  TextField
} from '@mui/material';

const Aktivnosti = () => {
  const { id } = useParams();
  const [aktivnost, setAktivnost] = useState([]);
  const [aktivnostZaIzmenu, setAktivnostZaIzmenu] = useState(null);

  useEffect(() => {
    async function fetchAktivnosti() {
      try {
        const response = await fetch(
          `https://localhost:7193/Aktivnosti/PreuzmiAktivnostiPutovanja/${id}`
        );
        const data = await response.json();
        setAktivnost(data);
      } catch (error) {
        console.error('Greška prilikom dohvatanja aktivnosti:', error);
      }
    }
    fetchAktivnosti();
  }, [id]);

const [izmenaPodataka, setIzmenaPodataka] = useState(false);
const [izmenjeniPodaci, setIzmenjeniPodaci] = useState({
  naziv: '',
  cena: 0
});

const [dodavanjeAktivno, setDodavanjeAktivno] = useState(false);
const [noviPodaci, setNoviPodaci] = useState({
    naziv: '',
    cena: 0
});

const handleDodajAktivnost = async () => {
  try {
    const response = await fetch('https://localhost:7193/Aktivnosti/DodajAktivnost', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(noviPodaci),
    });
    if (response.ok) {
      console.log('Nova aktivnost je dodata.');
      setAktivnost([...aktivnost, noviPodaci]);
      setDodavanjeAktivno(false); 
      setNoviPodaci({}); 
    } else {
      throw new Error('Greška prilikom dodavanja aktivnsoti.');
    }
  } catch (error) {
    console.error('Greška prilikom slanja zahteva za dodavanje aktivnosti:', error);
  }
};

const handleSacuvajIzmene = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(
        `https://localhost:7193/Aktivnosti/AzurirajAktivnost/${izmenjeniPodaci.id}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(izmenjeniPodaci),
        }
      );
      if (response.ok) {
        console.log(`Aktivnost sa ID ${izmenjeniPodaci.id} je ažurirana.`);

        setAktivnost((prevAktivnost) =>
        prevAktivnost.map((aktivnost) =>
            aktivnost.id === izmenjeniPodaci.id ? izmenjeniPodaci : aktivnost
          )
        );
        setAktivnostZaIzmenu(null); // Zatvaranje forme za izmenu
        setIzmenaPodataka(false);
      } else {
        throw new Error('Neuspelo ažuriranje aktivnosti.');
      }
    } catch (error) {
      console.error('Greška prilikom ažuriranja aktivnsoti:', error);
    }
  };
  

  const handleIzmeniAktivnost = (aktivnsotId) => {
    const izabranaAktivnost = aktivnost.find((aktivnost) => aktivnost.id === aktivnsotId);
    setAktivnostZaIzmenu(izabranaAktivnost);
    setIzmenjeniPodaci(izabranaAktivnost); // Postavljanje podataka za uređivanje
    setIzmenaPodataka(true); // Prikaz forme za izmenu
  };
  

  const handleObrisiAktivnost = async (aktivnostId) => {
    try {
      const response = await fetch(
        `https://localhost:7193/Aktivnosti/ObrisiAktivnost/${aktivnostId}`,
        {
          method: 'DELETE',
        }
      );
      if (response.ok) {
        console.log(`Aktivnsot sa ID ${aktivnostId} je obrisana.`);
        setAktivnost(aktivnost.filter((aktivnost) => aktivnost.id !== aktivnostId));
      } else {
        throw new Error('Neuspelo brisanje aktivnsoti.');
      }
    } catch (error) {
      console.error('Greška prilikom brisanja aktivnsoti:', error);
    }
  };

  return (
    <div style={{ textAlign: 'center', fontFamily: 'Roboto, sans-serif' }}>
      <Typography variant="h6" gutterBottom style={{ fontFamily: 'sans-serif' }}>
        <Divider style={{ marginTop: '30px' }}>Aktivnosti na putovanju</Divider>
      </Typography>
      <Grid container spacing={3} justifyContent="center">
        {aktivnost.map((aktivnost) => (
          <Grid item xs={12} sm={6} md={4} key={aktivnost.id}>
            <Card variant="outlined">
              <CardContent>
                <Typography variant="h6">{aktivnost.naziv}</Typography>
                <Typography variant="body6" color="textSecondary">
                  Cena: {aktivnost.cena}
                </Typography>
                <Button
                  variant="outlined"
                  color="secondary"
                  onClick={() => handleObrisiAktivnost(aktivnost.id)}
                >
                  Obriši
                </Button>
                <Button
                  variant="outlined"
                  color="primary"
                  onClick={() => handleIzmeniAktivnost(aktivnost.id)}
                  style={{ marginLeft: '10px' }}
                >
                  Izmeni
                </Button>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>
      {aktivnostZaIzmenu && izmenaPodataka && (
      <div style={{ textAlign: 'center', marginTop: '20px' }}>
        <Typography variant="h6">Izmeni aktivnost</Typography>
        <form onSubmit={handleSacuvajIzmene} style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <TextField
            label="Naziv"
            variant="outlined"
            value={izmenjeniPodaci.naziv}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, naziv: e.target.value })
            }
            style={{ marginBottom: '10px', width: '300px' }}
          />
          <TextField
            label="Cena"
            variant="outlined"
            type="number"
            value={izmenjeniPodaci.cena}
            onChange={(e) =>
              setIzmenjeniPodaci({ ...izmenjeniPodaci, cena: e.target.value })
            }
            style={{ marginBottom: '20px', width: '300px' }}
          />
          <Button type="submit" variant="contained" color="primary">
            Sačuvaj izmene
          </Button>
        </form>
      </div>
    )}
<Divider><Button
  variant="outlined"
  color="primary"
  onClick={() => setDodavanjeAktivno(true)}
  style={{ marginTop: '20px' }}
>
  Dodaj aktivnsot
</Button></Divider>


{dodavanjeAktivno && (
  <div style={{ textAlign: 'center', marginTop: '20px' }}>
    <Typography variant="h6">Dodaj aktivnost</Typography>
    <form
      onSubmit={(e) => {
        e.preventDefault();
        handleDodajAktivnost();
      }}
      style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}
    >
      <TextField
        label="Naziv"
        variant="outlined"
        value={noviPodaci.mesto}
        onChange={(e) =>
          setNoviPodaci({ ...noviPodaci, mesto: e.target.value })
        }
        style={{ marginBottom: '10px', width: '300px' }}
      />
      <TextField
        label="Cena"
        variant="outlined"
        value={noviPodaci.cena}
        onChange={(e) =>
          setNoviPodaci({ ...noviPodaci, cena: e.target.value })
        }
        style={{ marginBottom: '10px', width: '300px' }}
      />
      
      <Button type="submit" variant="contained" color="primary">
        Dodaj aktivnost
      </Button>
    </form>
  </div>
)}

    </div>
  );
};

export default Aktivnosti;





