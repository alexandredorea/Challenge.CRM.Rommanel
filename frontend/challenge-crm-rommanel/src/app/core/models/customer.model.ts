export interface AddressDto {
  postalCode:    string;
  street:        string;
  number:        string;
  neighborhood:  string;
  city:          string;
  federativeUnit: string;
}

export interface CustomerDto {
  id:                        string;
  name:                      string;
  documentNumber:            string;
  documentFormatted:         string;
  documentType:              'Individual' | 'LegalEntity';
  birthOrFoundationDate:     string;
  email:                     string;
  telephone:                 string;
  telephoneFormatted:        string;
  address:                   AddressDto;
  stateRegistration:         string | null;
  active:                    boolean;
  createdAt:                 string;
}

export interface CustomerEventDto {
  eventId:       string;
  eventType:     string;
  version:       number;
  userId:        string;
  correlationId: string;
  occurredAt:    string;
  payload:       unknown;
}

export interface CreateCustomerRequest {
  name:                      string;
  document:                  string;
  birthOrFoundationDate:     string;
  email:                     string;
  telephone:                 string;
  postalCode:                string;
  street:                    string;
  addressNumber:             string;
  neighborhood:              string;
  city:                      string;
  federativeUnit:            string;
  stateRegistration:         string | null;
}
