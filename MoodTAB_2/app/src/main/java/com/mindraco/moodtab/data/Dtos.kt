package com.mindraco.moodtab.data

data class LoginPayload(val Nombre: String, val Email: String, val EsFamiliar: Boolean)
data class LoginResponseDto(val Success: Boolean, val User: PacienteDto?)
data class PacienteDto(val ID_Paciente: Int, val Nombre: String, val Email: String, val IdUsuarioExterno: Int?)
