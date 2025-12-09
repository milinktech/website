# FOCUS Logistics Website - Project Regeneration Prompt

Use this comprehensive prompt to regenerate the entire project from scratch.

---

## 📋 Project Overview

Create a **Blazor WebAssembly** website for a logistics company called **"FOCUS"**. The site should be a modern, professional, and visually stunning single-page application with multiple routes.

### Technology Stack
- **Framework**: Blazor WebAssembly (.NET 8)
- **Styling**: Vanilla CSS (no TailwindCSS)
- **Icons**: Google Material Symbols Outlined
- **Fonts**: Inter (Google Fonts)
- **Project Name**: `FWW.Site.UI`

---

## 🎨 Design Requirements

### Brand Colors
| Role | Color |
|------|-------|
| Primary | `#0d59f2` (Blue) |
| Primary Dark | `#0A2342` (Navy) |
| Secondary/Accent | `#FF6700` (Orange) |

### Design Aesthetic
- Modern, premium feel with glassmorphism effects
- Smooth animations and hover effects
- Full dark mode support using `[data-theme="dark"]` CSS attribute
- Responsive design (mobile-first approach)
- Hero sections with gradient overlays on background images
- Card-based layouts with subtle shadows and hover lift effects

---

## 📁 Project Structure

```
src/FWW.Site.UI/
├── App.razor                  # Router configuration
├── Program.cs                 # Entry point
├── _Imports.razor             # Global using statements
├── FWW.Site.UI.csproj         # .NET 8 Blazor WASM project
├── Pages/
│   ├── Index.razor            # Homepage
│   ├── About.razor            # About Us page
│   ├── Services.razor         # Services page
│   ├── Contact.razor          # Contact page with form
│   └── Quote.razor            # Request a Quote multi-step form
├── Components/
│   ├── ServiceCard.razor      # Reusable service card
│   ├── StatCard.razor         # Statistics display
│   ├── TimelineItem.razor     # Timeline entry for history
│   ├── ValueCard.razor        # Core values card
│   └── ContactInfoCard.razor  # Contact information card
├── Shared/
│   ├── MainLayout.razor       # Main layout wrapper
│   ├── NavMenu.razor          # Header navigation
│   ├── Footer.razor           # Site footer
│   ├── ThemeToggle.razor      # Dark/light mode toggle
│   └── ChatPopup.razor        # AI chat popup (bottom-right)
└── wwwroot/
    ├── index.html             # HTML entry with font links
    └── css/app.css            # All styles (~1500 lines)
```

---

## 📄 Page Specifications

### 1. Homepage (`/`)
- **Hero Section**: Full-width background image with gradient overlay, tagline "Your Focus, Delivered Worldwide", CTA buttons for "Get a Free Quote" and "Track Your Shipment"
- **Services Grid**: 4 cards (Ocean Freight, Air Freight, Land Transport, Warehouse Solutions) using Material icons
- **Why Partner Section**: 2-column layout with feature images (Global Network, Advanced Technology)
- **Stats Section**: 3 statistics (195+ Countries Served, 2.5M+ Packages Delivered, 15+ Years Experience)
- **CTA Section**: Call-to-action with "Request a Quote" and "Contact Us" buttons

### 2. About Us (`/about`)
- **Hero Section**: Smaller hero with company tagline
- **Mission & Vision**: 2-column grid with mission and vision statements
- **Company Timeline**: 4 milestones (2005 Foundation, 2012 International Expansion, 2018 Tech Integration, 2023 Sustainability)
- **Core Values**: 3 cards (Reliability, Innovation, Customer-Centricity)
- **Leadership Team**: 4 team member cards with photos and titles
- **CTA Banner**: Dark background with "Get a Quote" button

### 3. Services (`/services`)
- **Hero Section**: Service-focused headline
- **Service Cards Grid**: 6 detailed service cards:
  1. Freight Forwarding (icon: `public`)
  2. Warehousing & Distribution (icon: `warehouse`)
  3. Supply Chain Management (icon: `hub`)
  4. Customs Brokerage (icon: `gavel`)
  5. Contract Logistics (icon: `handshake`)
  6. Last-Mile Delivery (icon: `local_shipping`)
- **CTA Banner**: Same as About page

### 4. Contact (`/contact`)
- **Hero Section**: Contact-focused headline
- **2-Column Layout**:
  - Left: Contact information cards (Phone, Email, Office Address)
  - Right: Contact form (First Name, Last Name, Email, Phone, Message)
- **Google Maps Embed**: Full-width iframe at bottom

### 5. Request a Quote (`/quote`)
- **Multi-Step Form** with progress indicator (Step 1 of 3)
- **Service Details Card**: Service Type dropdown, Shipping Date, Origin, Destination
- **Cargo Information Card**: Description, Weight, Dimensions (L/W/H), Hazardous goods radio
- **Contact Information Card**: Full Name, Email, Phone, Company, Additional Info
- **Submit Button**: "Get My Free Quote" with terms agreement text

---

## 🧩 Component Specifications

### Reusable Components (in `/Components`)

| Component | Parameters | Purpose |
|-----------|------------|---------|
| `ServiceCard` | `Icon`, `Title`, `Description` | Display service with icon |
| `StatCard` | `Value`, `Label` | Display statistic |
| `TimelineItem` | `Year`, `Title`, `Description` | Timeline milestone |
| `ValueCard` | `Icon`, `Title`, `Description` | Core value card |
| `ContactInfoCard` | `Icon`, `Title`, `Description`, `ContactInfo` | Contact detail with HTML support |

### Shared Components (in `/Shared`)

1. **NavMenu.razor**
   - Sticky header with blur backdrop
   - FOCUS logo (SVG icon + text)
   - Navigation links: Home, Services, About Us, Contact
   - Theme toggle button
   - "Get a Quote" CTA button
   - Mobile hamburger menu button

2. **Footer.razor**
   - 4-column grid: Company Info, Services Links, Company Links, Legal Links
   - Copyright with dynamic year

3. **ThemeToggle.razor**
   - Toggle between light/dark mode
   - Persists preference in localStorage
   - Updates `data-theme` attribute on `<html>`

4. **ChatPopup.razor** (AI Agent Ready)
   - Floating toggle button (bottom-right) with pulse animation
   - Chat window with header showing "FOCUS Assistant" with online status
   - Message list with user/bot styling and timestamps
   - Typing indicator animation
   - Text input with send button
   - Placeholder responses for common queries (track, quote, services, contact)
   - Ready for AI integration by replacing `GetPlaceholderResponse` method

---

## 🎭 CSS Design System

### CSS Custom Properties (Design Tokens)
```css
:root {
  /* Brand Colors */
  --color-primary: #0d59f2;
  --color-primary-hover: #0a47c2;
  --color-primary-dark: #0A2342;
  --color-secondary: #FF6700;
  
  /* Light Mode Defaults */
  --color-bg-primary: #ffffff;
  --color-bg-secondary: #f5f6f8;
  --color-card-bg: #ffffff;
  --color-text-primary: #333333;
  --color-text-secondary: #6b7280;
  --color-border: #e5e7eb;
}

[data-theme="dark"] {
  --color-bg-primary: #101622;
  --color-bg-secondary: #0d1117;
  --color-card-bg: #0d1117;
  --color-text-primary: #e5e7eb;
  --color-border: #374151;
}
```

### Key CSS Classes
- `.header` - Sticky nav with blur effect
- `.hero` - Full-width hero section with overlay
- `.section` - Standard page section with padding
- `.card` - Card with border, shadow, hover lift
- `.btn`, `.btn-primary`, `.btn-secondary`, `.btn-light` - Button variants
- `.form-group`, `.form-input`, `.form-select`, `.form-textarea` - Form elements
- `.grid`, `.grid-cols-*` - Responsive grid system

---

## 📱 Responsive Breakpoints
- **sm**: 640px
- **md**: 768px
- **lg**: 1024px
- **xl**: 1280px

---

## ✨ Animation Details

1. **Chat Toggle Button**: Pulse ring animation when closed
2. **Card Hover**: `translateY(-4px)` with increased shadow
3. **Button Hover**: Scale and shadow effects
4. **Chat Window**: Slide-up and scale animation on open
5. **Typing Indicator**: Bouncing dots animation
6. **Theme Toggle**: Rotate icon on hover

---

## 🔧 Key Implementation Notes

1. **No external CSS frameworks** - All styling is custom vanilla CSS
2. **Material Symbols Outlined** icons from Google Fonts CDN
3. **Forms use Blazor's EditForm** with InputText, InputSelect, InputTextArea, InputNumber, InputDate, InputRadioGroup components
4. **Navigation uses NavLink** with active state styling (`.active` class turns orange)
5. **Images use external URLs** from Google's public CDN (placeholder images)
6. **ChatPopup is designed for AI agent integration** - replace `GetPlaceholderResponse()` with actual API calls

---

## 📝 Sample Content

### Company Stats
- 195+ Countries Served
- 2.5M+ Packages Delivered
- 15+ Years of Experience

### Timeline Milestones
- 2005: Foundation of FOCUS
- 2012: First International Expansion (Singapore)
- 2018: Tech Integration (tracking platform)
- 2023: Sustainable Logistics Initiative

### Leadership Team
- John Carter - CEO
- Maria Rodriguez - COO
- David Chen - CTO
- Sarah Jenkins - Head of Global Partnerships

---

## 🚀 Build & Run

```bash
# Create project
dotnet new blazorwasm -n FWW.Site.UI -f net8.0

# Add packages
dotnet add package Microsoft.AspNetCore.Components.WebAssembly --version 8.0.0
dotnet add package Microsoft.AspNetCore.Components.WebAssembly.DevServer --version 8.0.0

# Run
dotnet run
```

---

## 📸 Reference Screenshots

The original designs were based on professional logistics company website mockups with:
- Clean, modern aesthetic
- Blue (#0d59f2) and orange (#FF6700) accent colors
- Large hero images with text overlays
- Card-based service displays
- Professional photography for team/fleet images

---

*This prompt contains all specifications needed to fully regenerate the FOCUS Logistics Blazor WebAssembly website.*
