import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Navbar from './Navbar';
import Footer from './Footer';
import Pocetna from './components/Pocetna';
import Agencije from './components/Agencije';
import Putovanje from './components/Putovanje';
import PutovanjeProfil from './components/PutovanjeProfil';

const App = () => {
  return (
    <Router>
      <Navbar />

      <Routes>
        <Route path="/" element={<Pocetna />} />
        <Route path="/Agencije" element={<Agencije />} />
        <Route path="/Agencije/:id" element={<Putovanje />} />
        <Route path="/Putovanje/:id" element={<PutovanjeProfil />} />
      </Routes>

      <Footer />
    </Router>
  );
};

export default App;