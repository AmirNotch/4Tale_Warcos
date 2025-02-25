package models

type StartGameRequest struct {
	GameRegimeEntryId string
	GameServerUrl     string
	UserToTeam        map[string]int
	UserToSquad       map[string]int
	TicketIds         []string
}

type CancelGameSearchRequest struct {
	TicketIds []string
}

type ScheduledGameRegime struct {
	GameMode          string
	GameRegime        string
	MapKind           string
	GameRegimeEntryId string
	IntervalStart     string
	IntervalEnd       string
	NumberOfPlayers   int32
	IsTeam            bool
}

type GameMode struct {
	GameModeKind    string
	NumberOfPlayers int32
	IsTeam          bool
	Created         string
}

type GameSettings struct {
	UserToTeam  map[string]int
	UserToSquad map[string]int
	GameMode    string
	GameRegime  string
	Map         string
	IsTeam      bool
}
