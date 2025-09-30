package com.mindraco.moodtab.viewmodel

import android.content.Context
import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.mindraco.moodtab.data.AuthService

class LoginViewModelFactory(private val authService: AuthService, private val ctx: Context) : ViewModelProvider.Factory {
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if (modelClass.isAssignableFrom(LoginViewModel::class.java)) {
            @Suppress("UNCHECKED_CAST")
            return LoginViewModel(authService, ctx.applicationContext) as T
        }
        throw IllegalArgumentException("Unknown ViewModel class")
    }
}