import React, { useState, useEffect } from 'react';
import { Link, Card, CardContent, Typography, Grid, CardActions, Button } from '@mui/material';
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

const PutovanjePrikaz = () => {
    const [putovanja, setPutovanja] = useState([]);

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('https://localhost:7193/Putovanje/PreuzmiPutovanja');
                const data = await response.json();
                setPutovanja(data);
            } catch (error) {
                console.error('Greška prilikom preuzimanja putovanja:', error);
            }
        };

        fetchData();
    }, []);

    return (
        <Grid container spacing={3}>
            {putovanja.map((putovanje) => (
                <Grid item key={putovanje.id} xs={12} sm={6} md={4}>
                    <StyledCard>
                        <StyledCardImage src={putovanje.slika} alt={putovanje.mesto}/>
                        <StyledCardContent>
                        <Typography variant="h6" component="div" sx={{ display: 'flex', justifyContent: 'space-between', paddingRight: '16px' }}>
                            {putovanje.mesto}
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
                        </StyledCardContent>
                    </StyledCard>
                </Grid>
            ))}
        </Grid>
    );
};

export default PutovanjePrikaz;